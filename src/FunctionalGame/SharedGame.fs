namespace FunctionalGame.SharedGame

open System

/// <summary>
/// EntityType
/// </summary> 
type EntityType =
    | Block
    
/// <summary>
/// Event
/// </summary>   
type Event =
    | EntitySpawned of int * EntityType * float32 * float32 * float32 * float32 * float32
    | EntityUpdated of int * float32 * float32 * float32
    | None
