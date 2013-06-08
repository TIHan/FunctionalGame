module FunctionalGame.ClientGame

open System
open System.IO
open System.Drawing
open System.Diagnostics
open System.Collections.Concurrent
open FunctionalGame.SharedGame
open FarseerPhysics

/// <summary>
/// Entity
/// </summary>   
type Entity = { Id: int; Type: EntityType; Width: float32; Height: float32; X: float32; Y: float32; Rotation: float32; Texture: Renderer.Texture }

/// <summary>
/// State
/// </summary>   
type State = { Entities: Map<int, Entity>; WindowHandle: int; }

/// <summary>
/// RenderEntity
/// </summary>   
let private RenderEntity (entity: Entity) =
    Renderer.RenderTexture entity.Texture entity.X entity.Y entity.Rotation

/// <summary>
/// GetTextureByEntityType
/// </summary>       
let private GetTextureByEntityType (entityType: EntityType) =
    match entityType with
    | _ -> Renderer.LoadTexture "Content/Textures/Block/BLOCK.png"
    
/// <summary>
/// EntitySpawned
/// </summary>
let private EntitySpawned (state: State) id entityType width height x y rotation =
    let texture = GetTextureByEntityType entityType
    let entity = {
        Id = id;
        Type = entityType;
        Width = ConvertUnits.ToDisplayUnits (float32 width);
        Height = ConvertUnits.ToDisplayUnits (float32 height);
        X = ConvertUnits.ToDisplayUnits (float32 x);
        Y = ConvertUnits.ToDisplayUnits (float32 y);
        Rotation = rotation * 180.f / float32 Math.PI;
        Texture = texture
    }
    { state with Entities = Map.add id entity state.Entities }
    
/// <summary>
/// EntityUpdated
/// </summary>   
let private EntityUpdated (state: State) id x y rotation =
    let entity = { Map.find id state.Entities with X = ConvertUnits.ToDisplayUnits (float32 x); Y = ConvertUnits.ToDisplayUnits (float32 y); Rotation = rotation * 180.f / float32 Math.PI }
    { state with Entities = Map.remove id state.Entities |> Map.add id entity }
    
/// <summary>
/// ProcessEvent
/// </summary>   
let private ProcessEvent state evt =
    match evt with
    
    | EntitySpawned (id, entityType, width, height, x, y, rotation) ->
        printfn "Entity %i spawned." id
        EntitySpawned state id entityType width height x y rotation
        
    | EntityUpdated (id, x, y, rotation) ->
        EntityUpdated state id x y rotation
        
    | _ -> state
    
/// <summary>
/// ProcessEvents
/// </summary> 
let rec ProcessEvents (evtQueue: ConcurrentQueue<Event>) state =
    match evtQueue.IsEmpty with
    | true -> state
    | _ ->
    
    let mutable evt: Event = None
    evtQueue.TryDequeue (&evt) |> ignore
    ProcessEvents evtQueue (ProcessEvent state evt)
    
/// <summary>
/// Init
/// </summary>       
let Init () =
    let windowHandle = Window.Create 1280 720 "Functional Game" 
    Renderer.Init ()
    { Entities = Map.empty; WindowHandle = windowHandle }
    
/// <summary>
/// Tick
/// </summary>   
let Tick (state: State) =
    Renderer.Clear ()
    Map.iter (fun id entity ->     
        RenderEntity entity
    ) state.Entities
    
    Window.Refresh state.WindowHandle
    state
    
/// <summary>
/// ShouldClose
/// </summary>       
let ShouldClose (state: State) =
    Window.ShouldClose state.WindowHandle
    