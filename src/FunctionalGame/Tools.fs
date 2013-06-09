module FunctionalGame.Tools

open System.Threading
open System.Diagnostics

let TimeLoop<'State> state (execution: 'State * Stopwatch -> 'State * bool) =
    let time = Stopwatch.StartNew ()
    let rec TimeLoopRec state =
        match execution (state, time) with
        | (newState, true) -> TimeLoopRec newState
        | _ -> ()
    TimeLoopRec state
