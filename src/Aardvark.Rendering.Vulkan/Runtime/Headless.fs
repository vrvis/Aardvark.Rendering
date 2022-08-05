﻿namespace Aardvark.Rendering.Vulkan

open System
open Aardvark.Base
open Aardvark.Rendering
open System.Runtime.InteropServices

type HeadlessVulkanApplication(debug : DebugLevel, instanceExtensions : list<string>, deviceExtensions : PhysicalDevice -> list<string>) =
    let requestedExtensions =
        [
            yield! instanceExtensions

            yield Instance.Extensions.ShaderSubgroupVote
            yield Instance.Extensions.ShaderSubgroupBallot
            yield Instance.Extensions.GetPhysicalDeviceProperties2

            yield! Instance.Extensions.Raytracing


            if debug > DebugLevel.None then
                yield Instance.Extensions.DebugReport
                yield Instance.Extensions.DebugUtils

            yield KHRGetPhysicalDeviceProperties2.Name
            yield KHRExternalMemoryCapabilities.Name
            yield KHRExternalMemory.Name
            yield KHRExternalFenceCapabilities.Name
            yield KHRExternalFence.Name
            yield KHRExternalSemaphoreCapabilities.Name
            yield KHRExternalSemaphore.Name
            yield EXTExternalMemoryHost.Name
            yield EXTExternalMemoryDmaBuf.Name

            if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
                yield KHRExternalMemoryWin32.Name
                yield KHRExternalFenceWin32.Name
                yield KHRExternalSemaphoreWin32.Name

            elif RuntimeInformation.IsOSPlatform OSPlatform.Linux then
                yield KHRExternalMemoryFd.Name
                yield KHRExternalFenceFd.Name
                yield KHRExternalSemaphoreFd.Name

        ]

    let requestedLayers =
        [
            if debug > DebugLevel.None then
                yield Instance.Layers.Validation
                yield Instance.Layers.AssistantLayer
        ]

    let instance = 
        let availableExtensions =
            Instance.GlobalExtensions |> Seq.map (fun e -> e.name) |> Set.ofSeq

        let availableLayers =
            Instance.AvailableLayers |> Seq.map (fun l -> l.name) |> Set.ofSeq

        // create an instance
        let enabledExtensions = requestedExtensions |> List.filter (fun r -> Set.contains r availableExtensions)
        let enabledLayers = requestedLayers |> List.filter (fun r -> Set.contains r availableLayers)
    
        new Instance(Version(1,1,0), enabledLayers, enabledExtensions)


    // choose a physical device
    let physicalDevice = 
        if instance.Devices.Length = 0 then
            failwithf "[Vulkan] could not get vulkan devices"
        else
            ConsoleDeviceChooser.run (CustomDeviceChooser.Filter instance.Devices)

    do instance.PrintInfo(Logger.Default, physicalDevice)

    // create a device
    let device = 
        let availableExtensions =
            physicalDevice.GlobalExtensions |> Seq.map (fun e -> e.name) |> Set.ofSeq

        let devExt = deviceExtensions physicalDevice
        let devExt = devExt |> List.filter (fun r -> Set.contains r availableExtensions)

        physicalDevice.CreateDevice(requestedExtensions @ devExt)

    // create a runtime
    let runtime = new Runtime(device, debug)

    do
        let dir =
            Path.combine [
                CachingProperties.CacheDirectory
                "Shaders"
                "Vulkan"
            ]
        runtime.ShaderCachePath <- Some dir

    member x.Dispose() =
        runtime.Dispose()
        device.Dispose()
        instance.Dispose()


    member x.Instance = instance
    member x.Device = device
    member x.Runtime = runtime

    new(debug : bool, instanceExtensions : list<string>, deviceExtensions : PhysicalDevice -> list<string>) =
        new HeadlessVulkanApplication(DebugLevel.ofBool debug, instanceExtensions, deviceExtensions)

    new() = new HeadlessVulkanApplication(DebugLevel.None, [], fun _ -> [])
    new(debug : DebugLevel) = new HeadlessVulkanApplication(debug, [], fun _ -> [])
    new(debug : bool) = new HeadlessVulkanApplication(debug, [], fun _ -> [])

    interface IDisposable with
        member x.Dispose() = x.Dispose()