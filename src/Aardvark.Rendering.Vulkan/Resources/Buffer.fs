﻿namespace Aardvark.Rendering.Vulkan

open System
open System.Threading
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Collections.Concurrent
open Aardvark.Base
open Aardvark.Rendering
open Microsoft.FSharp.NativeInterop

open Aardvark.Rendering.Vulkan.KHRBufferDeviceAddress
open Aardvark.Rendering.Vulkan.KHRAccelerationStructure

#nowarn "9"
// #nowarn "51"

// =======================================================================
// Resource Definition
// =======================================================================
type Buffer =
    class
        inherit Resource<VkBuffer>
        val public Memory : DevicePtr
        val public Usage : VkBufferUsageFlags
        val public Size : int64
        val public DeviceAddress : VkDeviceAddress

        override x.Destroy() =
            if x.Handle.IsValid && x.Size > 0L then
                VkRaw.vkDestroyBuffer(x.Device.Handle, x.Handle, NativePtr.zero)
                x.Handle <- VkBuffer.Null
                x.Memory.Dispose()

        interface IBackendBuffer with
            member x.Runtime = x.Device.Runtime :> IBufferRuntime
            member x.Handle = x.Handle :> obj
            member x.SizeInBytes = nativeint x.Size // NOTE: return size as specified by user. memory might have larger size as it is an aligned block

        new(device : Device, handle, memory, size, usage, refCount) =
            let address =
                if usage &&& VkBufferUsageFlags.ShaderDeviceAddressBitKhr <> VkBufferUsageFlags.None then
                    native {
                        let! pInfo = VkBufferDeviceAddressInfoKHR(handle)
                        return VkRaw.vkGetBufferDeviceAddressKHR(device.Handle, pInfo)
                    }
                else
                    0UL

            { inherit Resource<_>(device, handle, refCount); Memory = memory; Size = size; Usage = usage; DeviceAddress = address }

        new(device, handle, memory, size, usage) =
            new Buffer(device, handle, memory, size, usage, 1)
    end

type BufferView =
    class
        inherit Resource<VkBufferView>
        val public Buffer : Buffer
        val public Format : VkFormat
        val public Offset : uint64
        val public Size : uint64

        override x.Destroy() =
            if x.Handle.IsValid then
                VkRaw.vkDestroyBufferView(x.Device.Handle, x.Handle, NativePtr.zero)
                x.Handle <- VkBufferView.Null

        new(device, handle, buffer, fmt, offset, size) =
            { inherit Resource<_>(device, handle); Buffer = buffer; Format = fmt; Offset = offset; Size = size }
    end


// =======================================================================
// Command Extensions
// =======================================================================
[<AutoOpen>]
module BufferCommands =
    type Command with

        // buffer to buffer
        static member Copy(src : Buffer, srcOffset : int64, dst : Buffer, dstOffset : int64, size : int64) =
            if size < 0L || srcOffset < 0L || srcOffset + size > src.Size || dstOffset < 0L || dstOffset + size > dst.Size then
                failf "bad copy range"

            { new Command() with
                member x.Compatible = QueueFlags.All
                member x.Enqueue cmd =
                    let copyInfo = VkBufferCopy(uint64 srcOffset, uint64 dstOffset, uint64 size)
                    cmd.AppendCommand()
                    copyInfo |> pin (fun pInfo -> VkRaw.vkCmdCopyBuffer(cmd.Handle, src.Handle, dst.Handle, 1u, pInfo))
                    [src]
            }

        static member Copy(src : Buffer, dst : Buffer, ranges : Range1l[]) =
            if ranges.Length = 0 then
                Command.Nop
            else
                { new Command() with
                    member x.Compatible = QueueFlags.All
                    member x.Enqueue cmd =
                        let pCopyInfos = NativePtr.stackalloc ranges.Length
                        let mutable current = NativePtr.toNativeInt pCopyInfos
                        for r in ranges do
                            let ci = VkBufferCopy(uint64 r.Min, uint64 r.Min, uint64 (1L + r.Max - r.Min))
                            NativeInt.write current ci
                            current <- current + nativeint sizeof<VkBufferCopy>

                        cmd.AppendCommand()
                        VkRaw.vkCmdCopyBuffer(cmd.Handle, src.Handle, dst.Handle, uint32 ranges.Length, pCopyInfos)
                        [src; dst]
                }



        static member inline Copy(src : Buffer, dst : Buffer, size : int64) = 
            Command.Copy(src, 0L, dst, 0L, size)


        static member inline Copy(src : Buffer, dst : Buffer) = 
            Command.Copy(src, 0L, dst, 0L, min src.Size dst.Size)

        static member Acquire(buffer : Buffer, offset : int64, size : int64) =
            { new Command() with
                member x.Compatible = QueueFlags.All
                member x.Enqueue cmd =
                    cmd.AppendCommand()

                    let access = VkAccessFlags.ofBufferUsage buffer.Usage
                    let stage = VkAccessFlags.toDstStageFlags cmd.QueueFamily.Flags access

                    let barrier =
                        VkBufferMemoryBarrier(
                            VkAccessFlags.TransferWriteBit ||| VkAccessFlags.HostWriteBit ||| VkAccessFlags.MemoryWriteBit ||| VkAccessFlags.ShaderWriteBit,
                            access,
                            uint32 buffer.Device.TransferFamily.Index,
                            uint32 buffer.Device.GraphicsFamily.Index,
                            buffer.Handle,
                            uint64 offset,
                            uint64 size
                        )

                    barrier |> pin (fun pBarrier ->
                        VkRaw.vkCmdPipelineBarrier(
                            cmd.Handle,
                            VkPipelineStageFlags.TopOfPipeBit,
                            stage,
                            VkDependencyFlags.None,
                            0u, NativePtr.zero,
                            1u, pBarrier,
                            0u, NativePtr.zero
                        )
                    )

                    [buffer]
            }

        static member Acquire(buffer : Buffer) =
            Command.Acquire(buffer, 0L, buffer.Size)

        static member Sync(b : Buffer, src : VkAccessFlags, dst : VkAccessFlags) =
            { new Command() with
                member x.Compatible = QueueFlags.All
                member x.Enqueue cmd =
                    cmd.AppendCommand()
                    let barrier =
                        VkBufferMemoryBarrier(
                            src,
                            dst,
                            VK_QUEUE_FAMILY_IGNORED, VK_QUEUE_FAMILY_IGNORED,
                            b.Handle,
                            0UL,
                            uint64 b.Size
                        )

                    let srcStage = src |> VkAccessFlags.toSrcStageFlags cmd.QueueFamily.Flags
                    let dstStage = dst |> VkAccessFlags.toDstStageFlags cmd.QueueFamily.Flags

                    barrier |> pin (fun pBarrier ->
                        VkRaw.vkCmdPipelineBarrier(
                            cmd.Handle, 
                            srcStage, 
                            dstStage,
                            VkDependencyFlags.None,
                            0u, NativePtr.zero,
                            1u, pBarrier,
                            0u, NativePtr.zero
                        )
                    )

                    [b]
            }

        static member SyncWrite(b : Buffer) =
            { new Command() with
                member x.Compatible = QueueFlags.All
                member x.Enqueue cmd =
                    cmd.AppendCommand()
                    let barrier =
                        VkBufferMemoryBarrier(
                            VkAccessFlags.TransferWriteBit ||| VkAccessFlags.HostWriteBit ||| VkAccessFlags.MemoryWriteBit ||| VkAccessFlags.ShaderWriteBit,
                            VkAccessFlags.TransferReadBit ||| VkAccessFlags.ShaderReadBit ||| VkAccessFlags.IndexReadBit ||| VkAccessFlags.VertexAttributeReadBit ||| VkAccessFlags.UniformReadBit,
                            VK_QUEUE_FAMILY_IGNORED, VK_QUEUE_FAMILY_IGNORED,
                            b.Handle,
                            0UL,
                            uint64 b.Size
                        )
                    barrier |> pin (fun pBarrier ->
                        VkRaw.vkCmdPipelineBarrier(
                            cmd.Handle, 
                            VkPipelineStageFlags.TopOfPipeBit, 
                            VkPipelineStageFlags.BottomOfPipeBit,
                            VkDependencyFlags.None,
                            0u, NativePtr.zero,
                            1u, pBarrier,
                            0u, NativePtr.zero
                        )
                    )

                    [b]
            }

        static member ZeroBuffer(b : Buffer) =
            { new Command() with
                member x.Compatible = QueueFlags.All
                member x.Enqueue cmd =
                    cmd.AppendCommand()
                    VkRaw.vkCmdFillBuffer(cmd.Handle, b.Handle, 0UL, uint64 b.Size, 0u)
                    [b]
            }
        static member SetBuffer(b : Buffer, offset : int64, size : int64, value : byte[]) =
            { new Command() with
                member x.Compatible = QueueFlags.All
                member x.Enqueue cmd =
                    cmd.AppendCommand()
                    if value.Length <> 4 then failf "[Vulkan] pattern too long"
                    let v = BitConverter.ToUInt32(value, 0)
                    VkRaw.vkCmdFillBuffer(cmd.Handle, b.Handle, uint64 offset, uint64 size, v)
                    [b]
            }

    type CopyCommand with
        static member Copy(src : Buffer, srcOffset : int64, dst : Buffer, dstOffset : int64, size : int64) =
            CopyCommand.Copy(src.Handle, srcOffset, dst.Handle, dstOffset, size)

        static member Copy(src : Buffer, dst : Buffer, size : int64) =
            CopyCommand.Copy(src.Handle, 0L, dst.Handle, 0L, size)

        static member Copy(src : Buffer, dst : Buffer) =
            CopyCommand.Copy(src.Handle, 0L, dst.Handle, 0L, min src.Size dst.Size)

        static member Release(buffer : Buffer, offset : int64, size : int64, dstQueueFamily : DeviceQueueFamily) =
            CopyCommand.Release(buffer.Handle, offset, size, dstQueueFamily.Index)

        static member Release(buffer : Buffer, dstQueueFamily : DeviceQueueFamily) =
            CopyCommand.Release(buffer, 0L, buffer.Size, dstQueueFamily)

// =======================================================================
// Resource functions for Device
// =======================================================================
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Buffer =
    [<AutoOpen>]
    module private Helpers = 
        let memoryTypes (d : Device) (bits : uint32) =
            let mutable mask = 1u
            d.Memories 
            |> Seq.filter (fun m ->
                let mask = 1u <<< m.Info.index
                bits &&& mask <> 0u
               )
            |> Seq.toList

    let private emptyBuffers = ConcurrentDictionary<Device * VkBufferUsageFlags, Buffer>()

    let empty (usage : VkBufferUsageFlags) (device : Device) =
        let key = (device, usage)
        let buffer =
            emptyBuffers.GetOrAdd(key, fun (device, usage) ->
                let info =
                    VkBufferCreateInfo(
                        VkBufferCreateFlags.None,
                        256UL,
                        usage,
                        device.AllSharingMode,
                        device.AllQueueFamiliesCnt,
                        device.AllQueueFamiliesPtr
                    )

                let handle = 
                    info |> pin (fun pInfo ->
                        temporary (fun pHandle ->
                            VkRaw.vkCreateBuffer(device.Handle, pInfo, NativePtr.zero, pHandle)
                                |> check "could not create empty buffer"
                            NativePtr.read pHandle
                        )
                    )

                let reqs = 
                    temporary (fun ptr ->
                        VkRaw.vkGetBufferMemoryRequirements(device.Handle, handle, ptr)
                        NativePtr.read ptr
                    )

                let mem = device.Alloc(reqs, true)
                VkRaw.vkBindBufferMemory(device.Handle, handle, mem.Memory.Handle, uint64 mem.Offset)
                    |> check "could not bind empty buffer memory"


                device.OnDispose.Add (fun () ->
                    VkRaw.vkDestroyBuffer(device.Handle, handle, NativePtr.zero)
                    mem.Dispose()
                    emptyBuffers.TryRemove(key) |> ignore
                )

                new Buffer(device, handle, mem, 256L, usage)
            )

        buffer.AddReference()
        buffer

    let createConcurrent (conc : bool) (flags : VkBufferUsageFlags) (size : int64) (memory : DeviceHeap) =
        let device = memory.Device
        let info =
            VkBufferCreateInfo(
                VkBufferCreateFlags.None,
                uint64 size, 
                flags,
                (if conc then device.AllSharingMode else VkSharingMode.Exclusive),
                (if conc then device.AllQueueFamiliesCnt else 0u), 
                (if conc then device.AllQueueFamiliesPtr else NativePtr.zero)
            )

        let handle =
            info |> pin (fun pInfo ->
                temporary (fun pHandle ->
                    VkRaw.vkCreateBuffer(device.Handle, pInfo, NativePtr.zero, pHandle)
                        |> check "could not create buffer"
                    NativePtr.read pHandle
                )
            )
        let reqs =
            temporary (fun ptr ->   
                VkRaw.vkGetBufferMemoryRequirements(device.Handle, handle, ptr)
                NativePtr.read ptr
            )

        if reqs.memoryTypeBits &&& (1u <<< memory.Index) = 0u then
            failf "cannot create buffer using memory %A" memory

        let ptr = memory.Alloc(int64 reqs.alignment, int64 reqs.size)

        VkRaw.vkBindBufferMemory(device.Handle, handle, ptr.Memory.Handle, uint64 ptr.Offset)
            |> check "could not bind buffer-memory"


        new Buffer(device, handle, ptr, size, flags)

    let inline create  (flags : VkBufferUsageFlags) (size : int64) (memory : DeviceHeap) =
        createConcurrent false flags size memory

    let inline allocConcurrent (conc : bool) (flags : VkBufferUsageFlags) (size : int64) (device : Device) =
        createConcurrent conc flags size device.DeviceMemory

    let inline alloc (flags : VkBufferUsageFlags) (size : int64) (device : Device) =
        allocConcurrent false flags size device

    let internal ofWriter (flags : VkBufferUsageFlags) (size : nativeint) (writer : nativeint -> unit) (memory : DeviceHeap) =
        let device = memory.Device

        if size > 0n then
            let size = int64 size
            let buffer = memory |> create flags size

            if memory.IsHostVisible then
                buffer.Memory.Mapped (fun dst -> writer dst)

            else
                match device.UploadMode with
                | UploadMode.Sync ->
                    let hostBuffer = device.HostMemory |> create VkBufferUsageFlags.TransferSrcBit size
                    hostBuffer.Memory.Mapped (fun dst -> writer dst)

                    device.eventually {
                        try do! Command.Copy(hostBuffer, buffer)
                        finally hostBuffer.Dispose()
                    }

                | UploadMode.Async ->
                    let hostBuffer = device.HostMemory |> create VkBufferUsageFlags.TransferSrcBit size
                    hostBuffer.Memory.Mapped (fun dst -> writer dst)

                    device.CopyEngine.EnqueueSafe [
                        CopyCommand.Copy(hostBuffer, buffer, int64 size)
                        CopyCommand.Release(buffer, device.GraphicsFamily)
                        CopyCommand.Callback (fun () -> hostBuffer.Dispose())
                    ]

                    device.eventually {
                        do! Command.Acquire buffer
                    }

            buffer
        else
            empty flags device

    let internal updateRangeWriter (offset : int64) (size : int64) (writer : nativeint -> unit) (buffer : Buffer) =
        let device = buffer.Device

        if buffer.Memory.Memory.Heap.IsHostVisible then
            buffer.Memory.Mapped (fun dst -> writer (dst + nativeint offset))
        else
            let tmp = device.HostMemory |> create VkBufferUsageFlags.TransferSrcBit size
            tmp.Memory.Mapped (fun dst -> writer dst)

            device.eventually {
                try do! Command.Copy(tmp, 0L, buffer, offset, size)
                finally tmp.Dispose()
            }

    let internal updateWriter (writer : nativeint -> unit) (buffer : Buffer) =
        updateRangeWriter 0L buffer.Size writer buffer

    let uploadRanges (ptr : nativeint) (ranges : RangeSet) (buffer : Buffer) =
        let baseOffset = int64 ranges.Min
        let totalSize = int64 (ranges.Max - ranges.Min)

        buffer |> updateRangeWriter baseOffset totalSize (fun dst ->
            for r in ranges do
                let srcOffset = nativeint r.Min
                let dstOffset = nativeint (r.Min - ranges.Min)
                Marshal.Copy(ptr + srcOffset, dst + dstOffset, r.Size + 1)
        )

    let rec tryUpdate (data : IBuffer) (buffer : Buffer) =
        match data with
        | :? ArrayBuffer as ab ->
            let size = ab.Data.LongLength * int64 (Marshal.SizeOf ab.ElementType)
            if size = buffer.Size then
                let gc = GCHandle.Alloc(ab.Data, GCHandleType.Pinned)
                buffer |> updateWriter (fun ptr -> Marshal.Copy(gc.AddrOfPinnedObject(), ptr, size) )
                gc.Free()
                true
            else
                false

        | :? INativeBuffer as nb ->
            let size = nb.SizeInBytes |> int64
            if size = buffer.Size then
                nb.Use(fun src ->
                    buffer |> updateWriter (fun dst -> Marshal.Copy(src, dst, size))
                )
                true
            else
                false

        | :? IBufferRange as bv ->
            let handle = bv.Buffer
            tryUpdate handle buffer

        | _ ->
            false

    let rec ofBufferWithMemory (flags : VkBufferUsageFlags) (buffer : IBuffer) (memory : DeviceHeap) =
        let device = memory.Device

        match buffer with
        | :? ArrayBuffer as ab ->
            if ab.Data.Length <> 0 then
                let size = nativeint ab.Data.LongLength * nativeint (Marshal.SizeOf ab.ElementType)
                let gc = GCHandle.Alloc(ab.Data, GCHandleType.Pinned)
                try memory |> ofWriter flags size (fun dst -> Marshal.Copy(gc.AddrOfPinnedObject(), dst, size))
                finally gc.Free()
            else
                device |> empty flags

        | :? INativeBuffer as nb ->
            if nb.SizeInBytes <> 0 then
                let size = nb.SizeInBytes |> nativeint
                nb.Use(fun src ->
                    memory |> ofWriter flags size (fun dst -> Marshal.Copy(src, dst, size))
                )
            else
                device |> empty flags

        | :? Buffer as b ->
            b.AddReference()
            b

        | :? IBufferRange as bv ->
            let handle = bv.Buffer
            ofBufferWithMemory flags handle memory

        | _ ->
            failf "unsupported buffer type %A" buffer

    let rec ofBuffer (flags : VkBufferUsageFlags) (buffer : IBuffer) (device : Device) =
        device.DeviceMemory |> ofBufferWithMemory flags buffer



[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module BufferView =
    let create (fmt : VkFormat) (b : Buffer) (offset : uint64) (size : uint64) (device : Device) =
        if b.Size = 0L then
            new BufferView(device, VkBufferView.Null, b, fmt, offset, size)
        else
            let info =
                VkBufferViewCreateInfo(
                    VkBufferViewCreateFlags.None,
                    b.Handle,
                    fmt,
                    offset,
                    size
                )

            let handle = 
                info |> pin (fun pInfo ->
                    temporary (fun pHandle ->
                        VkRaw.vkCreateBufferView(device.Handle, pInfo, NativePtr.zero, pHandle)
                            |> check "could not create BufferView"
                        NativePtr.read pHandle
                    )
                )
            new BufferView(device, handle, b, fmt, offset, size)

// =======================================================================
// Device Extensions
// =======================================================================
[<AbstractClass; Sealed; Extension>]
type ContextBufferExtensions private() =

    [<Extension>]
    static member inline CreateBuffer(device : Device, flags : VkBufferUsageFlags, size : int64) =
        device |> Buffer.alloc flags size

    [<Extension>]
    static member inline CreateBuffer(device : Device, flags : VkBufferUsageFlags, b : IBuffer) =
        device |> Buffer.ofBuffer flags b

    [<Extension>]
    static member inline UploadRanges(buffer : Buffer, ptr : nativeint, ranges : RangeSet) =
        buffer |> Buffer.uploadRanges ptr ranges

    [<Extension>]
    static member inline TryUpdate(buffer : Buffer, b : IBuffer) =
        buffer |> Buffer.tryUpdate b

    [<Extension>]
    static member inline CreateBufferView(device : Device, buffer : Buffer, format : VkFormat, offset : int64, size : int64) =
        device |> BufferView.create format buffer (uint64 offset) (uint64 size)

[<AutoOpen>]
module ``Buffer Format Extensions`` = 
    module VkFormat =
        let ofType =
            LookupTable.lookupTable [
                typeof<float32>, VkFormat.R32Sfloat
                typeof<V2f>, VkFormat.R32g32Sfloat
                typeof<V3f>, VkFormat.R32g32b32Sfloat
                typeof<V4f>, VkFormat.R32g32b32a32Sfloat

                typeof<int>, VkFormat.R32Sint
                typeof<V2i>, VkFormat.R32g32Sint
                typeof<V3i>, VkFormat.R32g32b32Sint
                typeof<V4i>, VkFormat.R32g32b32a32Sint

                typeof<uint32>, VkFormat.R32Uint
                typeof<uint16>, VkFormat.R16Uint
                typeof<uint8>, VkFormat.R8Uint
                typeof<C4b>, VkFormat.B8g8r8a8Unorm
                typeof<C4us>, VkFormat.R16g16b16a16Unorm
                typeof<C4ui>, VkFormat.R32g32b32a32Uint
                typeof<C4f>, VkFormat.R32g32b32a32Sfloat
            ]
