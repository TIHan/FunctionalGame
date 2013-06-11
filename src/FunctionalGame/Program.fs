module FunctionalGame.Main

open System
open System.Threading
open System.Diagnostics
open FunctionalGame.SharedGame
    
type Game = {
    State: Game.State;
    ClientState: ClientGame.State;
    TotalTime: int64;
    ElapsedTime: int64;
    Frames: int;
}

let inline CalculateLerpAmount currentTime time rate =
    (currentTime - time) / rate

[<EntryPoint>]
let main args = 
    let rate = (1.f / 20.f * 1000.f)
    let clientRate = (1.f / 60.f * 1000.f)
    
    Tools.TimeLoop {
            State = Game.Init ();
            ClientState = ClientGame.Init ();
            TotalTime = int64 0;
            ElapsedTime = int64 0;
            Frames = 0;
        }
        
        (fun (game, time) ->
            match not (ClientGame.ShouldClose game.ClientState)  with
            | false -> (game, false)
            | _ ->
            let beginTime = time.ElapsedMilliseconds
            let timeToUpdate = float32 game.TotalTime + rate
            
            match float32 time.ElapsedMilliseconds >= timeToUpdate with
            | false ->                    
                let lerpAmount = float32 (time.ElapsedMilliseconds - game.TotalTime) / ((float32 game.TotalTime + rate) - float32 (game.TotalTime + game.ElapsedTime))
                let updatedClientState = game.ClientState |> ClientGame.ProcessEvents game.State.EventQueue |> ClientGame.Tick lerpAmount
                ({ game with ClientState = updatedClientState; Frames = game.Frames + 1 }, true)
            | _ ->
            
            let updatedState = 
                match game.State.EntitySpawnTime < time.ElapsedMilliseconds with
                | false -> game.State
                | _ ->
                    let newState = Game.SpawnEntity game.State EntityType.Block 16.f 16.f 5.f 0.f
                    { newState with EntitySpawnTime = time.ElapsedMilliseconds + int64 600 }
                |> Game.Tick
            
            printfn "%i" game.Frames
            ({ game with State = updatedState; TotalTime = beginTime; ElapsedTime = time.ElapsedMilliseconds - beginTime; Frames = 0 }, true)
    )
    0

