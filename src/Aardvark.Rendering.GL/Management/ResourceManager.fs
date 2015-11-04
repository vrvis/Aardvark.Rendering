﻿namespace Aardvark.Rendering.GL

open System
open System.Threading
open Aardvark.Base
open System.Collections.Concurrent
open Aardvark.Rendering.GL
open Aardvark.Rendering
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

[<AutoOpen>]
module ResourceManager =
    
    exception ResourceManagerException of string

    type internal ChangeableResourceDescription<'a> = 
        { trackChangedInputs : bool
          dependencies : list<IMod>; 
          updateCPU : list<IAdaptiveObject> -> unit; 
          updateGPU : unit -> unit; 
          destroy : unit -> unit; 
          resource : IMod<'a>;
          kind : ResourceKind }

    type IChangeableResource =
        inherit IDisposable
        inherit IAdaptiveObject
        //abstract member Dependencies : list<IMod>
        abstract member UpdateCPU : IAdaptiveObject -> unit
        abstract member UpdateGPU : IAdaptiveObject -> unit
        abstract member Resource : obj
        abstract member Kind : ResourceKind
        
    type ChangeableResource<'a> internal(key : list<obj>, parent : ConcurrentDictionary<list<obj>, obj>, 
                                         desc : IAdaptiveObject -> ChangeableResourceDescription<'a>) as this =
        inherit AdaptiveObject()

        static let updateCPUProbe = Symbol.Create "[Resource] update CPU"
        static let updateGPUProbe = Symbol.Create "[Resource] update GPU"

        let desc = desc this
        do desc.dependencies |> List.iter (fun a -> a.GetValue(this) |> ignore; a.AddOutput this)
           this.OutOfDate <- false

        let mutable isDisposed = false
        let mutable refCount = 1
        let mutable changedInputs = []

        override x.InputChanged (i : IAdaptiveObject) =
            if desc.trackChangedInputs then
                Interlocked.Change(&changedInputs, fun l -> i::l) |> ignore
                       

        //member x.Dependencies = desc.dependencies
        member x.UpdateCPU(caller : IAdaptiveObject) = 
            Telemetry.timed updateCPUProbe (fun () ->
                let changed = 
                    if desc.trackChangedInputs then Interlocked.Exchange(&changedInputs, [])
                    else []
                //desc.dependencies |> List.iter (fun a -> a.GetValue(x) |> ignore)
                desc.updateCPU changed
            )

        member x.UpdateGPU(caller) = 
            x.EvaluateIfNeeded (caller) () (fun () ->
                Telemetry.timed updateGPUProbe (fun () ->
                    //x.UpdateCPU(caller)
                    desc.updateGPU()
                )
            )

        member x.Resource = desc.resource
        member x.Kind = desc.kind

        member x.IncrementRefCount () = Interlocked.Increment &refCount

        member x.Dispose() =
            if parent = null then
                isDisposed <- true
                desc.destroy()
                desc.dependencies |> List.iter (fun a -> a.RemoveOutput this)

            elif not isDisposed then
                let r = Interlocked.Decrement &refCount
                if r = 0 then
                    isDisposed <- true
                    parent.TryRemove key |> ignore
                    desc.destroy()
                    desc.dependencies |> List.iter (fun a -> a.RemoveOutput this)

        interface IDisposable with
            member x.Dispose() = x.Dispose()

        interface IChangeableResource with
            //member x.Dependencies = desc.dependencies
            member x.UpdateCPU(caller) = x.UpdateCPU(caller)
            member x.UpdateGPU(caller) = x.UpdateGPU(caller)
            member x.Resource = desc.resource :> obj
            member x.Kind = desc.kind

        internal new(desc) = new ChangeableResource<'a>([], null, desc)

    let outOfDate (c : IChangeableResource) =
        c.OutOfDate

    let subscribeDirty (c : IChangeableResource) (set : System.Collections.Generic.HashSet<IChangeableResource>) (dirty : ref<int>) =
        let f() = dirty := 1; lock set (fun () -> set.Add c |> ignore)
        c.AddMarkingCallback(f)

    [<AutoOpen>]
    module private Caching =
        type ResourceCache() =
            let created = ConcurrentDictionary<list<obj>, obj>()

            member x.GetOrAdd(key : list<obj>, create : IAdaptiveObject -> ChangeableResourceDescription<'a>) =
                
                let res = 
                    created.AddOrUpdate(key, 
                        (fun key -> 
                            new ChangeableResource<'a>(key, created, create) :> obj),
                        fun key o ->
                            (o |> unbox<ChangeableResource<'a>>).IncrementRefCount () |> ignore
                            o
                    )

                let res = res |> unbox<ChangeableResource<'a>>

                res

        type NamedResourceCache() =
            let caches = ConcurrentDictionary<Symbol, ResourceCache>()

            member x.Item
                with get (name : Symbol) = caches.GetOrAdd(name, fun name -> ResourceCache())

    [<AutoOpen>]
    module private TypePatterns =


        let (|Array|_|) (t : Type) =
            if t.IsArray then
                let e = t.GetType().GetElementType()
                Array e |> Some
            else
                None

        let (|UntypedArray|_|) (t : Type) =
            if t = typeof<Array> then
                Some UntypedArray
            else
                None

        let (|ValueType|_|) (t : Type) =
            if t.IsValueType then
                Some ValueType
            else
                None

    module SurfaceCompilers =
        open System
        open System.Collections.Generic

        let compilers = Dictionary<Type,Context -> ISurface -> Error<Program>>()

        let registerShaderCompiler (compiler : Context -> 'a -> Error<Program>) =
            compilers.Add ( typeof<'a>, fun ctx s -> compiler ctx (unbox<'a> s) )

        let compileBackendSurface (ctx : Context) (b : BackendSurface) =
            match ctx.TryCompileProgram b.Code with
                | Success s ->
                    let remapSemantic (sem : string) =
                        match b.SemanticMap.TryGetValue (Sym.ofString sem) with
                            | (true, sem) -> sem.ToString()
                            | _ -> sem

                    
                    let getSamplerState (sem : string) =
                        match b.SamplerStates.ContainsKey (Sym.ofString sem) with
                            | true -> Some sem
                            | _ -> None

                    let ub = s.UniformBlocks |> List.map (fun b -> { b with fields = b.fields |> List.map (fun f -> { f with semantic = remapSemantic f.semantic }) })
                    let u = s.Uniforms |> List.map (fun f -> let sem = remapSemantic f.semantic in { f with semantic = sem; samplerState = getSamplerState sem})

                    let uniformGetters =
                        b.Uniforms 
                            |> SymDict.toSeq 
                            |> Seq.map (fun (k,v) -> (k, v :> obj)) 
                            |> SymDict.ofSeq

                    Success { s with UniformBlocks = ub; Uniforms = u; SamplerStates = b.SamplerStates; UniformGetters = uniformGetters }

                | Error e -> Error e

        do registerShaderCompiler compileBackendSurface

        do registerShaderCompiler (fun (ctx : Context) (g : IGeneratedSurface) -> 
            let b = g.Generate ctx.Runtime
            compileBackendSurface ctx b
           )

        let compile (ctx : Context) (s : ISurface) =   
            match compilers |> Seq.tryPick (fun ( KeyValue(k,v) ) -> 
                if k.IsAssignableFrom (s.GetType()) then Some <| v ctx s
                else None) with
             | Some k -> k
             | None -> Error "Unknown surface type. "

    module Sharing =
        open System.Collections.Generic

 
        type IResourceHandler<'d, 'r> =
            abstract member Create : 'd -> IMod<'r>
            abstract member Update : IMod<'r> * 'd -> unit
            abstract member Delete : IMod<'r> -> unit

        type SharedResource<'a>(key : obj, r : 'a) =
            let mutable refCount = 0
            let mutable key = key

            member x.ReferenceCount
                with get() = refCount
                and set c = refCount <- c

            member x.Resource = r

            member x.Key
                with get() = key
                and set (k : obj) = key <- k

        type SharedResourceView<'a> private(resource : SharedResource<'a>, handle : ModRef<'a>) =
            inherit AdaptiveDecorator(handle)
            let mutable resource = resource

            member x.SharedResource
                with get() = resource
                and set v = resource <- v

            member x.Handle = handle
            
            interface IMod with
                member x.IsConstant = false
                member x.GetValue(caller) = handle.GetValue(caller) :> obj

            interface IMod<'a> with
                member x.GetValue(caller) = handle.GetValue(caller)

            new(r : SharedResource<'a>) = SharedResourceView(r, Mod.init r.Resource)

        type SharedResourceHandler<'d, 'r>(create : 'd -> 'r, update : 'r * 'd -> unit, delete : 'r -> unit) =
            let cache = Dict<obj, SharedResource<'r>>()

            let getOrCreate (key : 'd) (creator : 'd -> 'r) =
                let res = 
                    cache.GetOrCreate(key, fun _ ->
                        let b = creator key
                        SharedResource(key,b)
                    )

                res.ReferenceCount <- res.ReferenceCount + 1
                SharedResourceView(res)

            member x.Create(arr : 'd) =
                getOrCreate arr (fun arr ->
                    create arr
                )

            member x.Update(b : SharedResourceView<'r>, arr : 'd) =
                let res = b.SharedResource

                if res.ReferenceCount <= 1 then  
                    // we're the only reference to that resource
                    cache.Remove res.Key |> ignore

                    match cache.TryGetValue arr with
                        | (true, r) ->
                            // there already is a resource for the given array
                            // therefore we destroy the old one
                            delete(res.Resource)

                            // and change the SharedResourceView accordingly
                            r.ReferenceCount <- r.ReferenceCount + 1
                            b.SharedResource <- r
                            transact (fun () -> b.Handle.Value <- r.Resource)
                            
                        | _ ->
                            // no other resource has the same key so we
                            // may simply upload and store the SharedResource at its
                            // new "location"
                            update (res.Resource, arr)
                            cache.[arr] <- res
                            res.Key <- arr
                else
                    res.ReferenceCount <- res.ReferenceCount - 1
                    match cache.TryGetValue arr with
                        | (true, r) ->
                            r.ReferenceCount <- r.ReferenceCount + 1

                            b.SharedResource <- r
                            transact (fun () -> b.Handle.Value <- r.Resource)

                        | _ ->
                            let r = SharedResource(arr, create arr)
                            r.ReferenceCount <- 1
                            cache.[arr] <- r

                            b.SharedResource <- r
                            transact (fun () -> b.Handle.Value <- r.Resource)

            member x.Delete(b : SharedResourceView<'r>) =
                let res = b.SharedResource

                b.SharedResource <- Unchecked.defaultof<_>
                b.Handle.UnsafeCache <- Unchecked.defaultof<_>

                if res.ReferenceCount <= 1 then
                    delete res.Resource
                    cache.Remove(res.Key) |> ignore
                else
                    res.ReferenceCount <- res.ReferenceCount - 1

            interface IResourceHandler<'d, 'r> with
                member x.Create(data) = x.Create(data) :> IMod<_>
                member x.Update(res, data) = x.Update(unbox res, data)
                member x.Delete(res) = x.Delete(unbox res)

        type NopResourceHandler<'d, 'r>(create : 'd -> 'r, update : 'r * 'd -> unit, delete : 'r -> unit) =
            interface IResourceHandler<'d, 'r> with
                member x.Create(data) = data |> create |> Mod.constant
                member x.Update(res, data) = update (res.GetValue(), data)
                member x.Delete(res) = delete (res.GetValue())


    [<AllowNullLiteral>]
    type ResourceManager(original : ResourceManager, ctx : Context, shareTextures : bool, shareBuffers : bool) =
        static let semanticIndices = ConcurrentDictionary<Symbol, int>()
        static let mutable currentId = -1

        static let getSemanticIndex (sem : Symbol) =
            semanticIndices.GetOrAdd(sem, fun s ->
                let id = Interlocked.Increment(&currentId)
                id
            )

        // some identifiers for caches
        static let arrayBuffer = Sym.ofString "ArrayBuffer"
        static let dataBuffer = Sym.ofString "DataBuffer"
        static let pixTexture = Sym.ofString "PixTexture"
        static let program = Sym.ofString "Program"
        static let uniformBuffer = Sym.ofString "UniformBuffer"
        static let sampler = Sym.ofString "Sampler"
        static let vao = Sym.ofString "VertexArrayObject"


        let bufferHandler = 
            if shareBuffers then 
                let originalHandler = 
                    match original with
                        | null -> None
                        | o when not o.ShareBuffers -> None
                        | o -> Some o.BufferHandler

                match originalHandler with
                    | Some h -> h
                    | None -> Sharing.SharedResourceHandler<IBuffer, Buffer>(ctx.CreateBuffer, ctx.Upload, ctx.Delete) :> Sharing.IResourceHandler<_,_>
            else 
                Sharing.NopResourceHandler<IBuffer, Buffer>(ctx.CreateBuffer, ctx.Upload, ctx.Delete) :> Sharing.IResourceHandler<_,_>

        let textureHandler = 
            if shareTextures then 
                let originalHandler = 
                    match original with
                        | null -> None
                        | o when not o.ShareTextures -> None
                        | o -> Some o.TextureHandler

                match originalHandler with
                    | Some h -> h
                    | None -> Sharing.SharedResourceHandler<ITexture, Texture>(ctx.CreateTexture, ctx.Upload, ctx.Delete) :> Sharing.IResourceHandler<_,_>
            else 
                Sharing.NopResourceHandler<ITexture, Texture>(ctx.CreateTexture, ctx.Upload, ctx.Delete) :> Sharing.IResourceHandler<_,_>


        // the overall cache holding caches per identifier
        let cache = 
            match original with
                | null -> NamedResourceCache()
                | o -> o.Cache

        let compile (s : ISurface) = SurfaceCompilers.compile ctx s

//        let volatileSubscribtion (m : IMod) (cb : (unit -> unit) -> unit) : IDisposable =
//            let f = ref Unchecked.defaultof<_>
//            let life = ref true
//            let current = ref null
//
//            let subscribe() =
//                if !life then
//                    lock m (fun () ->
//                        current := m.AddVolatileMarkingCallback !f
//                    )  
//                    
//            f := fun () ->
//                cb(subscribe)
//
//            subscribe()   
//
//            { new IDisposable with
//                member x.Dispose() =
//                    life := false
//                    if not (isNull !current) then
//                        current.Value.Dispose()
//                        current := null
//            } 
//            failwith "not impl"
//            let f = ref Unchecked.defaultof<_>
//            let life = ref true
//
//            let subscribe() =
//                if !life then
//                    lock m (fun () ->
//                        m.MarkingCallbacks.Add !f |> ignore
//                    )
//
//            f := fun () ->
//                cb(subscribe)
//
//            subscribe()
//
//            { new IDisposable with
//                member x.Dispose() =
//                    life := false
//                    m.MarkingCallbacks.Remove !f |> ignore
//            }
           
        member private x.BufferHandler = bufferHandler
        member private x.TextureHandler = textureHandler 
        member private x.Original = original
        member private x.Cache = cache
        member x.ShareBuffers = shareBuffers
        member x.ShareTextures = shareTextures 
        member x.Context = ctx

        /// <summary>
        /// creates a buffer from a mod-array simply updating its
        /// content whenever the array changes
        /// </summary>
        member x.CreateBuffer<'a when 'a : struct>(data : IMod<'a[]>) =
            cache.[arrayBuffer].GetOrAdd(
                [data], 
                fun self ->
                    let current = data.GetValue(self)
                    let handle = bufferHandler.Create(ArrayBuffer(current))
                    { trackChangedInputs = false
                      dependencies = [data]
                      updateCPU = fun _ -> data.GetValue(self) |> ignore
                      updateGPU = fun () -> bufferHandler.Update(handle, ArrayBuffer(data.GetValue(self)))
                      destroy = fun () -> bufferHandler.Delete(handle)
                      resource = handle
                      kind = ResourceKind.Buffer 
                    } 
            )

        /// <summary>
        /// creates a buffer from a mod-array simply updating its
        /// content whenever the array changes
        /// </summary>
        member x.CreateBuffer(data : IMod<IBuffer>) =
            
            cache.[arrayBuffer].GetOrAdd(
                [data], 
                fun self ->
                    let created = ref false
                    let current = data.GetValue(self)
                    let handle = 
                        match current with
                            | :? Buffer as bb ->
                                ref (Mod.constant bb)
                            | _ ->
                                created := true
                                ref (bufferHandler.Create(current))

                    let handleMod = Mod.init !handle

                    let updateTo (t : Buffer) =
                        if !created then
                            bufferHandler.Delete(!handle)
                            created := false

                        let h = Mod.constant t
                        handle := h
                        transact (fun () -> handleMod.Value <- h)

                    { trackChangedInputs = false
                      dependencies = [data]
                      updateCPU = fun _ -> data.GetValue(self) |> ignore
                      updateGPU = fun () ->
                        match data.GetValue(self) with
                            | :? Buffer as t -> updateTo t
                            | :? NullBuffer as n -> updateTo ( Buffer(ctx,0n,0) )
                            | _ -> 
                                if !created then
                                    bufferHandler.Update(!handle, data.GetValue(self))
                                else
                                    created := true
                                    handle := bufferHandler.Create(current)

                                if handleMod.Value <> !handle then 
                                    transact (fun () -> handleMod.Value <- !handle)
                      destroy = fun () -> if !created then bufferHandler.Delete(!handle)
                      resource = handleMod |> Mod.bind id
                      kind = ResourceKind.Buffer  }
            )



        /// <summary>
        /// creates a buffer from an untyped mod using one of the
        /// available overloads if possible. raises a ResourceManagerException
        /// if the given mod cannot be transformed into a buffer.
        /// </summary>       
        member x.CreateBuffer(data : IMod) =
            let t = data.GetType()
            match t with

                | ModOf(Array(ValueType as t)) ->
                    let m = typeof<ResourceManager>.GetMethod("CreateBuffer", [|typedefof<IMod<_>>.MakeGenericType [|t|] |])
                    m.Invoke(x, [|data :> obj|]) |> unbox<ChangeableResource<Buffer>>

                | ModOf(UntypedArray) ->
                    x.CreateBuffer( data |> unbox<IMod<Array>> |> Mod.map (fun arr -> ArrayBuffer(arr) :> IBuffer))

                | ModOf(t) when t = typeof<IBuffer> ->
                    x.CreateBuffer( data |> unbox<IMod<IBuffer>> )

                | _ ->
                    raise <| ResourceManagerException(sprintf "failed to create buffer for type: %A" t.FullName)
                    
     
        member x.CreateTexture(data : IMod<ITexture>) =
            cache.[pixTexture].GetOrAdd(
                [data],
                fun self ->
                    let current = data.GetValue(self)

                    let created = ref false
                    let handle = 
                        match current with
                            | :? Texture as t -> ref (Mod.constant t)
                            | _ -> 
                                created := true
                                ref <| textureHandler.Create(current)

                    let handleMod = Mod.init !handle

                    let updateTo (t : Texture) =
                        if !created then
                            textureHandler.Delete(!handle)
                            created := false

                        let h = Mod.constant t
                        handle := h
                        transact (fun () -> handleMod.Value <- h)

                    { trackChangedInputs = false
                      dependencies = [data]
                      updateCPU = fun _ -> data.GetValue(self) |> ignore
                      updateGPU = fun () -> 
                        match data.GetValue(self) with
                            | :? Texture as t -> updateTo t
                            | :? NullTexture as t -> updateTo Texture.empty
                            | _ -> 
                                if !created then
                                    textureHandler.Update(!handle, data.GetValue(self))
                                else
                                    created := true
                                    handle := textureHandler.Create(current)

                                if handleMod.Value <> !handle then 
                                    transact (fun () -> handleMod.Value <- !handle)

                      destroy = fun () -> if !created then textureHandler.Delete(!handle)
                      resource = handleMod |> Mod.bind id
                      kind = ResourceKind.Texture  }            
            )

        member x.CreateSurface (s : IMod<ISurface>) =
           cache.[program].GetOrAdd(
                [s],
                fun self ->
                    let current = s.GetValue(self)
                    match current with
                     | :? Program as p -> 
                        { trackChangedInputs = false
                          dependencies = [s]
                          updateCPU = ignore
                          updateGPU = id
                          destroy = id
                          resource = s |> Mod.map unbox
                          kind = ResourceKind.ShaderProgram  }    
                     | _ ->
                        let compileResult = compile current

                        match compileResult with
                            | Success p ->
                                let handle = Mod.init p

                                { trackChangedInputs = false
                                  dependencies = [s]
                                  updateCPU = fun _ -> 
                                    match compile <| s.GetValue(self) with
                                        | Success p -> Mod.change handle p
                                        | Error e -> Log.warn "could not update surface: %A" e

                                  updateGPU = fun () -> ()
                                  destroy = fun () -> ctx.Delete(p)
                                  resource = handle 
                                  kind = ResourceKind.ShaderProgram }         
                            | Error e ->
                                failwith e
           )
                
        member x.CreateUniformBuffer (scope : Ag.Scope, layout : UniformBlock, program : Program, u : IUniformProvider, semanticValues : byref<list<string * IMod>>) =
            let getValue (f : ActiveUniform) =
                let sem = f.semantic |> Sym.ofString

                match u.TryGetUniform (scope, sem) with
                    | Some m -> m
                    | _ ->
                        match program.UniformGetters.TryGetValue sem with
                            | (true, (:? IMod as m)) -> m
                            | _ ->
                                failwithf "could not find uniform: %A" f

            let fieldValues = layout.fields |> List.map (fun f -> f, getValue f)
            semanticValues <- fieldValues |> List.map (fun (f,v) -> f.semantic, v)
            let values = fieldValues |> List.map snd


            cache.[uniformBuffer].GetOrAdd(
                (layout :> obj)::(values |> List.map (fun v -> v :> obj)),
                fun self ->

                    let b = ctx.CreateUniformBuffer(layout)
                    
                    let writers = 
                        fieldValues 
                            |> List.map (fun (u,m) -> m :> IAdaptiveObject, b.CompileSetter (Sym.ofString u.name) m)
                            |> List.toArray
  
                    
                    writers |> Array.iter (fun (_,w) -> w(self))
                    ctx.Upload(b)

                    if writers.Length > 4 then
                        let writers = Dictionary.ofArray writers
                        { trackChangedInputs = true
                          dependencies = values
                          updateCPU = fun changed ->
                            let mutable w = Unchecked.defaultof<_>
                            for c in changed do
                                if writers.TryGetValue(c, &w) then
                                    w(self)
                            

                          updateGPU = fun () -> ctx.Upload(b)
                          destroy = fun () -> 
                            ctx.Delete(b)
                          resource = Mod.constant b
                          kind = ResourceKind.UniformBuffer  }    
                    else
                        let writers = writers |> Array.map snd
                        { trackChangedInputs = false
                          dependencies = values
                          updateCPU = fun _ ->
                            for w in writers do w(self)
                          updateGPU = fun () -> ctx.Upload(b)
                          destroy = fun () -> 
                            ctx.Delete(b)
                          resource = Mod.constant b
                          kind = ResourceKind.UniformBuffer  }   
            )

        member x.CreateUniformLocation(scope : Ag.Scope, u : IUniformProvider, uniform : ActiveUniform) =
            match u.TryGetUniform (scope, Sym.ofString uniform.semantic) with
                | Some v ->
                    cache.[uniformBuffer].GetOrAdd(
                        [uniform :> obj; v :> obj],
                        fun self ->
                            let loc = ctx.CreateUniformLocation(uniform.uniformType.SizeInBytes, uniform.uniformType)

                            let write = loc.CompileSetter(v)
                            let writer = [v :> IAdaptiveObject] |> Mod.mapCustom (fun s -> write(s))
                            
                            writer.GetValue(self)

                            { trackChangedInputs = false
                              dependencies = [v]
                              updateCPU = fun _ ->
                                writer.GetValue(self)

                              updateGPU = fun () -> ()
                              destroy = fun () -> ()
                              resource = Mod.constant loc
                              kind = ResourceKind.UniformBuffer  }    
                    )
                | _ ->
                    failwithf "could not get uniform: %A" uniform

        member x.CreateSampler (sam : IMod<SamplerStateDescription>) =
            cache.[sampler].GetOrAdd(
                [sam], 
                fun self ->
                    let current = sam.GetValue(self)
                    let handle = ctx.CreateSampler(current)

                    { trackChangedInputs = true
                      dependencies = [sam]
                      updateCPU = fun _ -> sam.GetValue(self) |> ignore
                      updateGPU = fun () -> ctx.Update(handle, sam.GetValue(self))
                      destroy = fun () -> ctx.Delete(handle)
                      resource = Mod.constant handle 
                      kind = ResourceKind.SamplerState }
            )

        member x.CreateVertexArrayObject (bindings : list<int * IMod<AttributeDescription>>, index : ChangeableResource<Buffer>) =
            cache.[vao].GetOrAdd(
                [bindings; index], 
                fun self ->

                    let handle = ctx.CreateVertexArrayObject(index.Resource.GetValue(self), bindings |> List.map (fun (i,r) -> i, r.GetValue(self)))
                    let attributes = bindings |> List.map snd |> List.map (fun b -> b :> IMod)

                    { trackChangedInputs = true
                      dependencies = (index.Resource :> IMod)::attributes
                      updateCPU = fun _ -> ()
                      updateGPU = fun () -> ctx.Update(handle, index.Resource.GetValue(self), bindings |> List.map (fun (i,r) -> i, r.GetValue(self)))
                      destroy = fun () -> ctx.Delete(handle)
                      resource = Mod.constant handle 
                      kind = ResourceKind.VertexArrayObject
                    }
            )

        member x.CreateVertexArrayObject (bindings : list<int * IMod<AttributeDescription>>) =
            cache.[vao].GetOrAdd(
                [bindings], 
                fun self ->

                    let handle = ctx.CreateVertexArrayObject(bindings |> List.map (fun (i,r) -> i, r.GetValue(self)))
                    let attributes = bindings |> List.map snd |> List.map (fun b -> b :> IMod)

                    { trackChangedInputs = true
                      dependencies = attributes
                      updateCPU = fun _ -> ()
                      updateGPU = fun () -> ctx.Update(handle,bindings |> List.map (fun (i,r) -> i, r.GetValue(self)))
                      destroy = fun () -> ctx.Delete(handle)
                      resource = Mod.constant handle
                      kind = ResourceKind.VertexArrayObject 
                    }
            )

        member x.CreateVertexArrayObject (bindings : list<int * IMod<AttributeDescription>>, index : Option<ChangeableResource<Buffer>>) =
            match index with
                | Some index -> x.CreateVertexArrayObject(bindings, index)
                | None -> x.CreateVertexArrayObject(bindings)

        member x.CreateTexture(size : IMod<V2i>, mipLevels : IMod<int>, format : IMod<PixFormat>, samples : IMod<int>) : ChangeableResource<Texture> =
            
            let textureFormat =
                Mod.map2 (fun pf mips -> TextureFormat.ofPixFormat pf { TextureParams.empty with wantMipMaps = mips > 1 }) format mipLevels
            
            
            let desc self =
                let handle = ctx.CreateTexture2D(size.GetValue(self), mipLevels.GetValue(self), textureFormat.GetValue(self), samples.GetValue(self))
                
                { trackChangedInputs = true
                  dependencies = [size :> IMod; textureFormat :> IMod; samples :> IMod]
                  updateCPU = fun _ -> ()
                  updateGPU = fun () -> ctx.UpdateTexture2D(handle, size.GetValue(self), mipLevels.GetValue(self), textureFormat.GetValue(self), samples.GetValue(self))
                  destroy = fun () -> ctx.Delete(handle)
                  resource = Mod.constant handle
                  kind = ResourceKind.Texture
                }

            new ChangeableResource<Texture>(desc)

        member x.CreateRenderbuffer(size : IMod<V2i>, format : IMod<RenderbufferFormat>, samples : IMod<int>) : ChangeableResource<Renderbuffer> =
            
            let desc self =
                let handle = ctx.CreateRenderbuffer(size.GetValue(self), format.GetValue(self), samples.GetValue(self))
                { trackChangedInputs = true
                  dependencies = [size :> IMod; format :> IMod; samples :> IMod]
                  updateCPU = fun _ -> ()
                  updateGPU = fun () -> ctx.Update(handle, size.GetValue(self), format.GetValue(self), samples.GetValue(self))
                  destroy = fun () -> ctx.Delete(handle)
                  resource = Mod.constant handle 
                  kind = ResourceKind.Renderbuffer
                }

            new ChangeableResource<Renderbuffer>(desc)

        member x.CreateFramebuffer(bindings : list<Symbol * IMod<IFramebufferOutput>>) : ChangeableResource<Aardvark.Rendering.GL.Framebuffer> =
            let dict = SymDict.ofList bindings

            let desc self =
                let toInternal (bindings : list<Symbol * IMod<IFramebufferOutput>>) =
                
                    let depth =
                        match dict.TryGetValue DefaultSemantic.Depth with
                            | (true, d) ->
                                Some <| d.GetValue(self)
                            | _ ->
                                None

                    let colors = bindings |> List.filter (fun (s,b) -> s <> DefaultSemantic.Depth) |> List.map (fun (s,o) -> getSemanticIndex s, s, (o.GetValue(self)))

                    colors,depth

                let c,d = toInternal bindings
                let handle = ctx.CreateFramebuffer(c, d)

                { trackChangedInputs = true
                  dependencies = bindings |> Seq.map (fun (_,v) -> v :> IMod) |> Seq.toList
                  updateCPU = fun _ -> ()
                  updateGPU = fun () -> 
                    let c,d = toInternal bindings
                    ctx.Update(handle, c, d)
                  destroy = fun () -> ctx.Delete(handle)
                  resource = Mod.constant handle
                  kind = ResourceKind.Framebuffer 
                }

            new ChangeableResource<Aardvark.Rendering.GL.Framebuffer>(desc)

        new(ctx, shareTextures, shareBuffers) = ResourceManager(null, ctx, shareTextures, shareBuffers)