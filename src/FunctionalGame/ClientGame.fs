module FunctionalGame.ClientGame

open System
open System.IO
open System.Drawing
open System.Diagnostics

type ClientEntity = { Width: float32; Height: float32; X: float32; Y: float32; Rotation: float32; TextureHandle: uint32 }

type ClientState = { Entities: ClientEntity list; WindowHandle: uint32; }
    
let Init () =
    let wid = Window.Create 1280 720 "Functional Game" 
    Renderer.Init ()
    { Entities = List.Empty; WindowHandle = wid } 
