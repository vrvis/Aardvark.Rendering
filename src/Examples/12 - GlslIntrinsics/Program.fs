﻿open Aardvark.Base
open Aardvark.Base.Rendering
open Aardvark.Base.Incremental
open Aardvark.SceneGraph
open Aardvark.Application

// This example illustrates how to render a simple triangle using aardvark.

module Shader =
    open FShade 

    [<ReflectedDefinition>]
    [<GLSLIntrinsic("gl_FrontFacing")>]
    let frontFace () : bool = raise <| FShade.Imperative.FShadeOnlyInShaderCodeException "frontFace"

    let test (v : Effects.Vertex) =
        fragment {
            if frontFace () then return V4d(1.0,0.0,0.0,1.0)
            else return V4d(0.0,1.0,0.0,1.0)
        }

[<EntryPoint>]
let main argv = 
    
    // first we need to initialize Aardvark's core components
    Ag.initialize()
    Aardvark.Init()

    // then we define some vertex attributes for our triangle
    let positions = [| V3f(-0.5f, -0.5f, 0.0f); V3f(0.5f, -0.5f, 0.0f); V3f(0.0f, 0.5f, 0.0f) |]
    let colors    = [| C4b.Red; C4b.Green; C4b.Blue |]
    
    let sg = 
        // create a scenegraph rendering a triangle-list
        Sg.draw IndexedGeometryMode.TriangleList
            
            // apply the attributes we have (position, color) to the draw-call
            // NOTE that aardvark figures out how many triangles are rendered automatically
            //      in this simple case (Sg.render provides a more flexible API for creating draw-calls)
            |> Sg.vertexAttribute DefaultSemantic.Positions (Mod.constant positions)
            |> Sg.vertexAttribute DefaultSemantic.Colors (Mod.constant colors)

            // apply a simple shader defined in the core libraries
            // interpolating per-vertex colors for each fragment 
            |> Sg.shader {
                do! DefaultSurfaces.vertexColor
                do! Shader.test
            }
    

    // show the scene in a simple window
    show {
        backends [Backend.GL; Backend.Vulkan; Backend.Vulkan]
        display Display.Mono
        debug false
        samples 8
        scene sg
    }

    0
