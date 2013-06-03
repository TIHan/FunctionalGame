module FunctionalGame.ClientGame

open System
open System.IO
open System.Drawing
open System.Diagnostics
open FunctionalGame.SharedGame

type ClientEntity = { Id: int; Type: EntityType; Width: float32; Height: float32; X: float32; Y: float32; Rotation: float32; Texture: Renderer.Texture }

type ClientState = { Entities: Map<int, ClientEntity>; WindowHandle: int; }

let private RenderEntity (entity: ClientEntity) =
    Renderer.RenderTexture entity.Texture entity.X entity.Y entity.Rotation
    
let Init () =
    let windowHandle = Window.Create 1280 720 "Functional Game" 
    Renderer.Init ()
    { Entities = Map.empty; WindowHandle = windowHandle }
   

let Tick (state: ClientState) =
    Renderer.Clear ()
    Map.iter (fun id entity ->     
        RenderEntity entity
    ) state.Entities
    
    Window.Refresh state.WindowHandle
    state
    
    
let ShouldClose (state: ClientState) =
    Window.ShouldClose state.WindowHandle


let private GetTextureByEntityType (entityType: EntityType) =
    match entityType with
    | _ -> Renderer.LoadTexture "Content/Textures/Block/BLOCK.png"

// Events

/// <summary>
/// EntitySpawned
/// </summary>
let EntitySpawned (state: ClientState) id entityType width height x y rotation =
    let texture = GetTextureByEntityType entityType
    let entity = { Id = id; Type = entityType; Width = width; Height = height; X = x; Y = y; Rotation = rotation; Texture = texture }
    { state with Entities = Map.add id entity state.Entities }
    
/// <summary>
/// EntityUpdated
/// </summary>   
let EntityUpdated (state: ClientState) id width height x y rotation =
    let entity = Map.find id state.Entities
    { state with Entities = Map.remove id state.Entities |> Map.add id entity }
    