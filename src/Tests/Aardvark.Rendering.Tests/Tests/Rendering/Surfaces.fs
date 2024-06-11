﻿namespace Aardvark.Rendering.Tests.Rendering

open Aardvark.Rendering
open Aardvark.Rendering.Tests
open Aardvark.Application
open FSharp.Data.Adaptive
open Expecto
open FShade

module Surfaces =

    module Cases =

        let private glDynamicShaderCaching (signature: IFramebufferSignature) (surface: Surface) (runtime: GL.Runtime) =
            use __ = runtime.Context.ResourceLock
            let _, p1 = runtime.ResourceManager.CreateSurface(signature, surface, IndexedGeometryMode.TriangleList)
            let _, p2 = runtime.ResourceManager.CreateSurface(signature, surface, IndexedGeometryMode.TriangleList)
            Expect.isTrue (obj.ReferenceEquals(p1, p2)) "Not reference equal"

        let private vkDynamicShaderCaching (pass: Vulkan.RenderPass) (surface: Surface) (runtime: Vulkan.Runtime) =
            let _, p1 = runtime.ResourceManager.CreateShaderProgram(pass, surface, IndexedGeometryMode.TriangleList)
            let _, p2 = runtime.ResourceManager.CreateShaderProgram(pass, surface, IndexedGeometryMode.TriangleList)

            p1.Acquire()
            p2.Acquire()

            try
                Expect.isTrue (obj.ReferenceEquals(p1, p2)) "Not reference equal"
            finally
                p1.Release()
                p2.Release()

        let dynamicShaderCaching (runtime: IRuntime) =
            use signature =
                runtime.CreateFramebufferSignature([
                    DefaultSemantic.Colors, TextureFormat.Rgba8
                ])

            let surface =
                let effect =
                    Effect.compose [
                        Effects.Trafo.Effect
                        Effects.VertexColor.Effect
                    ]

                let compile (cfg: EffectConfig) =
                    let module_ = Effect.toModule cfg effect
                    let layout = EffectInputLayout.ofModule module_
                    layout, (AVal.init module_) :> aval<_>

                Surface.FShade compile

            match runtime, signature with
            | :? GL.Runtime as r, _ -> glDynamicShaderCaching signature surface r
            | :? Vulkan.Runtime as r, (:? Vulkan.RenderPass as p) -> vkDynamicShaderCaching p surface r
            | _ -> failwith "Unknown backend"

    let tests (backend : Backend) =
        [
            "Dynamic shader caching", Cases.dynamicShaderCaching
        ]
        |> prepareCases backend "Surfaces"