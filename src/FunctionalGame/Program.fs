module FunctionalGame.Main

open System
open System.Threading
open System.Diagnostics
open FunctionalGame.SharedGame
    
type GameState = { State: Game.State; ClientState: ClientGame.State; Time: int64 }

let CalculateLerpAmount rate time currentTime =
    (currentTime - time) / rate

[<EntryPoint>]
let main args = 
    
    let gameState = { State = Game.Init (); ClientState = ClientGame.Init (); Time = int64 0 }
    let rate = (1.f / 20.f * 1000.f)
    
    Tools.TimeLoop gameState (fun (state, time) ->
        match not (ClientGame.ShouldClose state.ClientState)  with
        | false -> (state, false)
        | _ ->
        let currentTime = time.ElapsedMilliseconds
        let timeToUpdate = float32 state.Time + rate
        
        match float32 time.ElapsedMilliseconds >= timeToUpdate with
        | false ->
            let updatedClientState = state.ClientState |> ClientGame.ProcessEvents state.State.EventQueue
            let lerpAmount = CalculateLerpAmount rate (float32 state.Time) (float32 time.ElapsedMilliseconds)

            ({ state with ClientState = updatedClientState |> ClientGame.Tick lerpAmount }, true)
        | _ ->
        
        let changedState = 
            match state.State.EntitySpawnTime < time.ElapsedMilliseconds with
            | false -> state.State
            | _ ->
                let newState = Game.SpawnEntity state.State EntityType.Block 16.f 16.f 5.f 0.f
                { newState with EntitySpawnTime = time.ElapsedMilliseconds + int64 600 }
            |> Game.Tick
        
        let elapsedTime = time.ElapsedMilliseconds - currentTime
        
        ({ state with State = changedState; Time = currentTime }, true)
    )
    0

