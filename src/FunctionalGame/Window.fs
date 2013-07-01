module FunctionalGame.Window

open System
open OpenF.GLFW


let Create width height title =
    GLFW.Init ()
    
#if OPENGL_2_1
    GLFW.WindowHint WindowHintTarget.ContextVersionMajor 2 |> ignore
    GLFW.WindowHint WindowHintTarget.ContextVersionMinor 1 |> ignore
#else   
    #if OPENGL_3_3
    GLFW.WindowHint WindowHintTarget.ContextVersionMajor 3 |> ignore
    GLFW.WindowHint WindowHintTarget.ContextVersionMinor 2 |> ignore
    GLFW.WindowHint WindowHintTarget.OpenGLForwardCompatible 1 |> ignore
    GLFW.WindowHint WindowHintTarget.OpenGLProfile WindowHintValue.OpenGLCoreProfile |> ignore
    #else
    raise (Exception "GL Context not specified.")
    #endif
#endif
    

    let handle = GLFW.CreateWindow width height "Functional Game" (0)//GLFW.GetPrimaryMonitor ())
    GLFW.MakeContextCurrent handle
    GLFW.SwapInterval (3)
    handle
    

let ShouldClose handle =
    GLFW.WindowShouldClose handle

        
let Refresh handle =
    GLFW.SwapBuffers handle
    GLFW.PollEvents ()
    

let Close () =
    GLFW.Terminate ()
    