module FunctionalGame.Main

open System
open System.IO
open System.Drawing
open System.Diagnostics
open System.Threading
open System.Collections.Generic
open FarseerPhysics
open FarseerPhysics.Common
open FarseerPhysics.Collision
open FarseerPhysics.Dynamics
open OpenF.GL
open OpenF.GLFW
    
type Game = { State: Game.State; ClientProcess: Tools.Process<ClientGame.ClientState, ClientGame.ClientMessage>  }

[<EntryPoint>]
let main args = 
    
    let game = { State = Game.Init (); ClientProcess = ClientGame.ClientProcess }
    
    //let tid = Renderer.LoadBlock ()
    //let floorY = ConvertUnits.ToSimUnits (720.f)
    //CreateFloor world 0.0f floorY
    
    game.ClientProcess.Start ()
    
    Tools.TimeLoop game (fun (state, time) ->
        // not (ClientGame.ShouldClose state.ClientState)
        match true  with
        | false -> (state, false)
        | _ ->  
        
        let startTime = time.ElapsedMilliseconds

        // TODO: Message passing required.
        let newState = Game.Tick state.State
        
        game.ClientProcess.Sync<unit> (fun x -> ClientGame.Tick x)
        
        let endTime = time.ElapsedMilliseconds
        let processTime = (1.f / 60.f * 1000.f) - float32 (endTime - startTime)
        Console.WriteLine (processTime)
        if processTime > 0.f then
            Thread.Sleep (int processTime)
            
        match state.State.EntitySpawnTime < time.ElapsedMilliseconds with
        | true ->
            ({state with State = { newState with EntitySpawnTime = time.ElapsedMilliseconds + int64 1000 }; }, true)
        | _ ->    
        (state, true)
    )
            
    GLFW.Terminate ()
    0

