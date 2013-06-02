module FunctionalGame.Tools

open System.Diagnostics

type Process<'State, 'Msg> (initial: 'State, execute) =
    let mailbox = new MailboxProcessor<'Msg> (fun agent ->
        let rec loop (state: 'State)  =
            async {
                let! msg = agent.Receive ()
                return! loop (execute state msg)
            }
        loop initial)
        
    member this.Start () =
        mailbox.Start ()

    member this.Send msg =
        mailbox.Post msg
    
    member this.SendAndAwait<'Reply> msg : 'Reply =
        mailbox.PostAndReply msg
        
    member this.TrySendAndAwait<'Reply> msg timeout : Option<'Reply> =
        mailbox.TryPostAndReply (msg, timeout)


let TimeLoop<'State> state (execution: 'State * Stopwatch -> 'State * bool) =
    let time = Stopwatch.StartNew ()
    let rec TimeLoopRec state =
        match execution (state, time) with
        | (newState, true) -> TimeLoopRec newState
        | _ -> ()
    TimeLoopRec state
