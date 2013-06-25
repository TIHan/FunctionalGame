module FunctionalGame.Main

open System
open System.Threading
open System.Diagnostics
open FunctionalGame.SharedGame
    
type Game = {
    State: Game.State;
    ClientState: ClientGame.State;
    LastTickTime: int64;
    NextTickTime: int64;
    ElapsedTickTime: int64;
    NextRenderTickTime: int64;
    EventQueue: Event list
}

[<EntryPoint>]
let main args = 
    let rate = (1.f / 20.f * 1000.f)
    let clientRate = (1.f / 60.f * 1000.f)
    
    Tools.TimeLoop {
            State = Game.Init ();
            ClientState = ClientGame.Init ();
            LastTickTime = int64 0;
            NextTickTime = int64 0;
            ElapsedTickTime = int64 0;
            NextRenderTickTime = int64 0;
            EventQueue = list.Empty;
        }
        
        (fun (game, time) ->
            match not (ClientGame.ShouldClose game.ClientState)  with
            | false -> (game, false)
            | _ ->
            let tickTime = time.ElapsedMilliseconds
            
            match tickTime >= game.NextTickTime with
            | false -> // Game Client Update
                match tickTime >= game.NextRenderTickTime with
                | false -> (game, true)
                | _ ->
                let lerpAmount = float32 (tickTime - game.LastTickTime) / rate
                let updatedClientState = game.ClientState |> ClientGame.ProcessEvents game.EventQueue |> ClientGame.Tick lerpAmount
                ({ game with ClientState = updatedClientState; NextRenderTickTime = game.NextRenderTickTime + int64 clientRate; EventQueue = [] }, true)
                
            | _ -> // Game Update                  
            let updatedState = game.State |> Game.Tick tickTime
            let nextTickTime = tickTime + int64 rate
            let elapsedTime = time.ElapsedMilliseconds - tickTime
            let eventQueue = game.EventQueue @ updatedState.EventQueue
            let renderTickTime = elapsedTime + int64 clientRate
            printfn "%i" elapsedTime
            ({ game with State = updatedState; LastTickTime = tickTime; NextTickTime = nextTickTime;
                        ElapsedTickTime = elapsedTime; NextRenderTickTime = renderTickTime; EventQueue = eventQueue }, true)
    )
    0

