module FunctionalGame.Window

open System
open OpenF.GLFW

let Create width height title =
    GLFW.Init ()

    let wid = GLFW.CreateWindow 1280 720 "Functional Game"
    GLFW.MakeContextCurrent wid
    wid
    
let Refresh wid =
    GLFW.SwapBuffers wid
    GLFW.PollEvents ()
    