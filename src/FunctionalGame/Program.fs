module FunctionalGame.Main

open System
open System.Threading
open FunctionalGame.SharedGame
    
type GameState = { State: Game.State; ClientState: ClientGame.State  }

[<EntryPoint>]
let main args = 
    
    let gameState = { State = Game.Init (); ClientState = ClientGame.Init () }
    
    Tools.TimeLoop gameState (fun (state, time) ->
        match not (ClientGame.ShouldClose state.ClientState)  with
        | false -> (state, false)
        | _ ->  
        
        let startTime = time.ElapsedMilliseconds
        
        let changedState = Game.Tick state.State
        let changedClientState = state.ClientState |> ClientGame.Tick |> ClientGame.ProcessEvents changedState.EventQueue
        
        let endTime = time.ElapsedMilliseconds
        let processTime = (1.f / 60.f * 1000.f) - float32 (endTime - startTime)
        printfn "%f" processTime
        if processTime > 0.f then
            Thread.Sleep (int processTime)
            
        match state.State.EntitySpawnTime < time.ElapsedMilliseconds with
        | true ->
            let newState = Game.SpawnEntity changedState EntityType.Block 16.f 16.f 5.f 0.f
            ({ state with State = { newState with EntitySpawnTime = time.ElapsedMilliseconds + int64 1000 }; ClientState = changedClientState }, true)
        | _ -> ({ state with State = changedState; ClientState = changedClientState }, true)
    )
    0

