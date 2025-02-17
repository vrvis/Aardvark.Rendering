﻿namespace Aardvark.Rendering.GL

open System
open OpenTK.Graphics
open OpenTK.Graphics.OpenGL4

[<AutoOpen>]
module ARB_buffer_storage =

    type GL private() =

        static let supported = ExtensionHelpers.isSupported (Version(4,4)) "GL_ARB_buffer_storage"

        static member ARB_buffer_storage = supported

    type GL.Dispatch with

        static member BufferStorage(target : BufferTarget, size : nativeint, data : nativeint, flags: BufferStorageFlags) =
            if GL.ARB_buffer_storage then
                GL.BufferStorage(target, size, data, flags)
            else
                GL.BufferData(target, size, data, BufferUsageHint.DynamicDraw)