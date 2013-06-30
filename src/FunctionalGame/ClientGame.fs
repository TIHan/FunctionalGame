module FunctionalGame.ClientGame

open System
open System.IO
open System.Drawing
open System.Diagnostics
open System.Collections.Concurrent
open FunctionalGame.SharedGame
open FunctionalGame.Renderer
open FarseerPhysics

/// <summary>
/// Entity
/// </summary>   
type Entity = {
    Id: int;
    Type: EntityType;
    Width: float32;
    Height: float32;
    X: float32;
    Y: float32;
    Rotation: float32;
    LastX: float32;
    LastY: float32;
    LastRotation: float32;
    Texture: Renderer.Texture;
}

/// <summary>
/// State
/// </summary>   
type State = {
    Rate: float32;
    Entities: Map<int, Entity>;
    WindowHandle: int;
    Time: Stopwatch;
}

/// <summary>
/// Lerp
/// </summary>  
let private Lerp value1 value2 amount =
    match amount with
    | x when x <= 0.f -> value1
    | x when x >= 1.f -> value2
    | _ -> value1 + (value2 - value1) * amount
    
let internal Renderer : IRenderer = new GL21Renderer () :> IRenderer

/// <summary>
/// RenderEntity
/// </summary>   
let private RenderEntity (renderer: IRenderer) (entity: Entity) lerpAmount =
    let x = Lerp entity.LastX entity.X lerpAmount
    let y = Lerp entity.LastY entity.Y lerpAmount
    let rotation = Lerp entity.LastRotation entity.Rotation lerpAmount
    renderer.RenderTexture (entity.Texture, x, y, rotation)

/// <summary>
/// GetTextureByEntityType
/// </summary>       
let private GetTextureByEntityType (renderer: IRenderer) (entityType: EntityType) =
    match entityType with
    | _ -> renderer.LoadTexture "Content/Textures/Block/BLOCK.png"
    
/// <summary>
/// EntitySpawned
/// </summary>
let private EntitySpawned (state: State) id entityType width height x y rotation =
    let texture = GetTextureByEntityType Renderer entityType
    let entity = {
        Id = id;
        Type = entityType;
        Width = ConvertUnits.ToDisplayUnits (float32 width);
        Height = ConvertUnits.ToDisplayUnits (float32 height);
        X = ConvertUnits.ToDisplayUnits (float32 x);
        Y = ConvertUnits.ToDisplayUnits (float32 y);
        Rotation = rotation * 180.f / float32 Math.PI;
        LastX = ConvertUnits.ToDisplayUnits (float32 x);
        LastY = ConvertUnits.ToDisplayUnits (float32 y);
        LastRotation = rotation * 180.f / float32 Math.PI;
        Texture = texture;
    }
    { state with Entities = Map.add id entity state.Entities }
    
/// <summary>
/// EntityUpdated
/// </summary>   
let private EntityUpdated (state: State) id x y rotation =
    let foundEntity = Map.find id state.Entities
    let lastX = foundEntity.X
    let lastY = foundEntity.Y
    let lastRotation = foundEntity.Rotation
    let entity = {
        foundEntity with X = ConvertUnits.ToDisplayUnits (float32 x); Y = ConvertUnits.ToDisplayUnits (float32 y); Rotation = rotation * 180.f / float32 Math.PI; LastX = lastX; LastY = lastY; LastRotation = lastRotation
    }
    { state with Entities = Map.remove id state.Entities |> Map.add id entity }
    
/// <summary>
/// ProcessEvent
/// </summary>   
let private ProcessEvent state evt =
    match evt with
    
    | EntitySpawned (id, entityType, width, height, x, y, rotation) ->
        EntitySpawned state id entityType width height x y rotation
        
    | EntityUpdated (id, x, y, rotation) ->
        EntityUpdated state id x y rotation
        
    | _ -> state
    
/// <summary>
/// ProcessEvents
/// </summary> 
let rec ProcessEvents (eventQueue: Event list) state =
    match eventQueue with
    | [] -> state
    | head :: tail ->
        ProcessEvents tail (ProcessEvent state head)
    
/// <summary>
/// Init
/// </summary>       
let Init () =
    let windowHandle = Window.Create 1280 720 "Functional Game" 
    
    Renderer.Init ()
    {
        Rate = 1.f / 60.f * 1000.f;
        Entities = Map.empty;
        WindowHandle = windowHandle;
        Time = Stopwatch.StartNew ();
    }
    
/// <summary>
/// Tick
/// </summary>   
let Tick lerpAmount (state: State) =
    Renderer.Clear ()
    Map.iter (fun id entity ->     
        RenderEntity Renderer entity lerpAmount
    ) state.Entities
    
    Window.Refresh state.WindowHandle
    state
    
/// <summary>
/// ShouldClose
/// </summary>       
let ShouldClose (state: State) =
    Window.ShouldClose state.WindowHandle
    