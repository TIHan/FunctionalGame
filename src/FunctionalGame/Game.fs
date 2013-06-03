module FunctionalGame.Game

open System
open System.IO
open System.Drawing
open System.Diagnostics
open System.Threading
open System.Collections.Generic
open FunctionalGame.SharedGame

type Entity = { Id: int; Type: EntityType; Physics: Physics.PhysicsObject }

type State = { Entities: Map<int, Entity>; Counter: int; EntitySpawnTime: int64; PhysicsEngine: Physics.PhysicsEngine }
    
    
let Init () =
    { Entities = Map.empty; Counter = 0; EntitySpawnTime = int64 0; PhysicsEngine = new Physics.PhysicsEngine (0.f, 9.82f) }
    
    
let Tick (state: State) =
    state.PhysicsEngine.Tick (1.f / 60.f)
    let entities = state.Entities |> Map.map (fun id entity ->
        { entity with Physics = state.PhysicsEngine.GetUpdatedObject entity.Physics }
    )
    { state with Entities = entities }


