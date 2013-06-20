module FunctionalGame.Game

open System
open System.IO
open System.Drawing
open System.Diagnostics
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
    EventQueue: Event list
}

/// <summary>
/// SpawnEntity
/// </summary>
let private SpawnEntity (state: State) entityType width height x y =
    let entity = { Id = state.Counter; Type = entityType; Physics = state.PhysicsEngine.CreateObject width height x y }
    let evt = EntitySpawned (entity.Id, entity.Type, width, height, x, y, 0.f)
    { state with Counter = state.Counter + 1; Entities = Map.add entity.Id entity state.Entities; EventQueue = state.EventQueue @ [evt] }
    
    
let private UpdateEntity (state: State) entity =
    { entity with Physics = state.PhysicsEngine.GetUpdatedObject entity.Physics }

/// <summary>
/// GameLogic
/// </summary>    
let private GameLogic tickTime (state: State) =
    match state.EntitySpawnTime < tickTime with
    | false -> state
    | _ ->
        let newState = SpawnEntity state EntityType.Block 16.f 16.f 5.f 0.f
        { newState with EntitySpawnTime = tickTime + int64 1000 }
        
/// <summary>
/// TickPhysics
/// </summary>         
let private TickPhysics (state: State) =
    state.PhysicsEngine.Tick (1.f / 20.f)
    
    let entities = state.Entities |> Map.map (fun id entity ->
        UpdateEntity state entity
    )
    
    let events = state.Entities |> Map.map (fun id entity ->
                        EntityUpdated (entity.Id, entity.Physics.X, entity.Physics.Y, entity.Physics.Rotation)
                    )
                |> Map.toList |> List.map (fun (id, entity) -> entity)
    
    { state with Entities = entities; EventQueue = state.EventQueue @ events }
    
    
/// <summary>
/// Init
/// </summary>       
let Init () =
    { Entities = Map.empty; Counter = 0; EntitySpawnTime = int64 0; PhysicsEngine = new Physics.PhysicsEngine (0.f, 9.82f); EventQueue = [] }

    
/// <summary>
/// Tick
/// </summary>       
let Tick tickTime (state: State) =
    { state with EventQueue = [] } |> GameLogic tickTime |> TickPhysics

