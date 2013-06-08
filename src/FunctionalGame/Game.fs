module FunctionalGame.Game

open System
open System.IO
open System.Drawing
open System.Diagnostics
open System.Threading
open System.Collections.Generic
open System.Collections.Concurrent
open FunctionalGame.SharedGame

/// <summary>
/// Entity
/// </summary>   
type Entity = { Id: int; Type: EntityType; Physics: Physics.PhysicsObject }

/// <summary>
/// State
/// </summary>   
type State = {
    Entities: Map<int, Entity>;
    Counter: int;
    EntitySpawnTime: int64;
    PhysicsEngine: Physics.PhysicsEngine;
    EventQueue: ConcurrentQueue<Event>
}

/// <summary>
/// SpawnEntity
/// </summary>
let SpawnEntity (state: State) entityType width height x y =
    let entity = { Id = state.Counter; Type = entityType; Physics = state.PhysicsEngine.CreateObject width height x y }
    state.EventQueue.Enqueue (EntitySpawned (entity.Id, entity.Type, width, height, x, y, 0.f))
    { state with Counter = state.Counter + 1; Entities = Map.add entity.Id entity state.Entities }
    
/// <summary>
/// Init
/// </summary>       
let Init () =
    { Entities = Map.empty; Counter = 0; EntitySpawnTime = int64 0; PhysicsEngine = new Physics.PhysicsEngine (0.f, 9.82f); EventQueue = new ConcurrentQueue<Event> () }
    
/// <summary>
/// Tick
/// </summary>       
let Tick (state: State) =
    state.PhysicsEngine.Tick (1.f / 60.f)
    let entities = state.Entities |> Map.map (fun id entity ->
        let updated = { entity with Physics = state.PhysicsEngine.GetUpdatedObject entity.Physics }
        state.EventQueue.Enqueue (EntityUpdated (updated.Id, updated.Physics.X, updated.Physics.Y, updated.Physics.Rotation))
        updated
    )
    { state with Entities = entities }


