module FunctionalGame.Window

open System
open OpenF.GLFW


let Create width height title =
    GLFW.Init ()

    let handle = GLFW.CreateWindow 1280 720 "Functional Game" (0) // GLFW.GetPrimaryMonitor ()
    GLFW.MakeContextCurrent handle
    handle
    

let ShouldClose handle =
    GLFW.WindowShouldClose handle

        
let Refresh handle =
    GLFW.SwapBuffers handle
    GLFW.PollEvents ()
    

let Close () =
    GLFW.Terminate ()
    