module FunctionalGame.Main

open System
open System.IO
open System.Drawing
open System.Diagnostics
open System.Threading
open System.Collections.Generic
open FarseerPhysics
open FarseerPhysics.Common
open FarseerPhysics.Collision
open FarseerPhysics.Dynamics
open OpenF.GL
open OpenF.GLFW
    
      
let CreateDynamicFixture world x y =
    let position = Nullable<Microsoft.Xna.Framework.Vector2> (new Microsoft.Xna.Framework.Vector2(x, y))
    let body = new Body (world, position)
    let width = ConvertUnits.ToSimUnits(8.f)
    let height = ConvertUnits.ToSimUnits(8.f)
    let shape = new Shapes.PolygonShape (PolygonTools.CreateRectangle(width, height), 1.0f)
    let fixture = body.CreateFixture shape
    
    fixture.Restitution <- 0.0f
    fixture.Body.BodyType <- BodyType.Dynamic
    fixture.Body.Mass <- 1.0f
    fixture.Body.SleepingAllowed <- true
    fixture  


let CreateFloor world x y =
    let position = Nullable<Microsoft.Xna.Framework.Vector2> (new Microsoft.Xna.Framework.Vector2 (x, y))
    let body = new Body (world, position)    
    let shape = new Shapes.PolygonShape (PolygonTools.CreateRectangle(1000.0f, 0.1f), 1.f)
    body.CreateFixture shape |> ignore


type Entity = { Fixture: Fixture; }

type State = { Entities: Entity list; EntitySpawnTime: int64 }

[<EntryPoint>]
let main args = 
    let clientState = ClientGame.Init ()
    
    let entities: Entity list = list.Empty
    
    let world = new World (new Microsoft.Xna.Framework.Vector2 (0.f, 9.82f))
        
    let SpawnEntity entities =
        let fixture = CreateDynamicFixture world 5.f 0.f
        let entity = { Fixture = fixture; }
        entities @ [entity]
    
    let tid = Renderer.LoadBlock ()
    let floorY = ConvertUnits.ToSimUnits (720.f)
    CreateFloor world 0.0f floorY
    
    Tools.TimeLoop { Entities = entities; EntitySpawnTime = int64 0 } (fun (state, time) ->
        match not (GLFW.WindowShouldClose clientState.WindowHandle) with
        | false -> (state, false)
        | _ ->  
        
        let startTime = time.ElapsedMilliseconds

        world.Step (1.f / 60.f)
        
        GL.Clear (ClearMask.ColorBufferBit)
        
        List.iter (fun entity ->     
            let x = ConvertUnits.ToDisplayUnits (entity.Fixture.Body.Position.X)
            let y = ConvertUnits.ToDisplayUnits (entity.Fixture.Body.Position.Y)
            let rotation = (entity.Fixture.Body.Rotation * 180.f / float32 Math.PI)
            Renderer.RenderBlock x y rotation tid
        ) state.Entities
        
        GLFW.SwapBuffers clientState.WindowHandle
        GLFW.PollEvents ()
        
        let endTime = time.ElapsedMilliseconds
        let processTime = (1.f / 60.f * 1000.f) - float32 (endTime - startTime)
        Console.WriteLine (processTime)
        if processTime > 0.f then
            Thread.Sleep (int processTime)
            
        match state.EntitySpawnTime < time.ElapsedMilliseconds with
        | true ->
            ({ Entities = SpawnEntity state.Entities; EntitySpawnTime = time.ElapsedMilliseconds + int64 100 }, true)
        | _ ->    
        (state, true)
    )
            
    GLFW.Terminate ()
    0

