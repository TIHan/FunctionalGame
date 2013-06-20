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
    EventQueue: Event list
}

[<EntryPoint>]
let main args = 
    let rate = (1.f / 20.f * 1000.f)
    
    Tools.TimeLoop {
            State = Game.Init ();
            ClientState = ClientGame.Init ();
            LastTickTime = int64 0;
            NextTickTime = int64 0;
            EventQueue = list.Empty;
        }
        
        (fun (game, time) ->
            match not (ClientGame.ShouldClose game.ClientState)  with
            | false -> (game, false)
            | _ ->
            let tickTime = time.ElapsedMilliseconds
            
            match tickTime >= game.NextTickTime with
            | false -> // Game Client Update
                let lerpAmount = float32 (tickTime - game.LastTickTime) / rate
                let updatedClientState = game.ClientState |> ClientGame.ProcessEvents game.EventQueue |> ClientGame.Tick lerpAmount
                ({ game with ClientState = updatedClientState; EventQueue = [] }, true)
                
            | _ -> // Game Update                  
            let updatedState = game.State |> Game.Tick tickTime
            let random = new Random (int tickTime)
            ({ game with State = updatedState; LastTickTime = tickTime; NextTickTime = tickTime + int64 rate; EventQueue = game.EventQueue @ updatedState.EventQueue }, true)
    )
    0

