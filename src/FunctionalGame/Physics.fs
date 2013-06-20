module FunctionalGame.Physics

open System
open System.Diagnostics
open System.Collections.Generic
open FarseerPhysics
open FarseerPhysics.Common
open FarseerPhysics.Collision
open FarseerPhysics.Dynamics
open FarseerPhysics.Factories


let private CreateDynamicFixture world displayWidth displayHeight x y =
    let width = ConvertUnits.ToSimUnits (displayWidth / 2.f)
    let height = ConvertUnits.ToSimUnits (displayHeight / 2.f)
    let body = BodyFactory.CreateBody (world, new Microsoft.Xna.Framework.Vector2 (x, y))
    let shape = new Shapes.PolygonShape (PolygonTools.CreateRectangle (width, height), 1.0f)
    let fixture = body.CreateFixture shape
    
    fixture.Restitution <- 0.4f
    fixture.Body.BodyType <- BodyType.Dynamic
    fixture.Body.Mass <- 1.0f
    fixture.Body.SleepingAllowed <- true
    fixture  


let private CreateFloor world x y =
    let body = BodyFactory.CreateBody (world, new Microsoft.Xna.Framework.Vector2 (x, y))  
    let shape = new Shapes.PolygonShape (PolygonTools.CreateRectangle(1000.0f, 0.1f), 1.f)
    body.CreateFixture shape |> ignore


type PhysicsObject = { Id: int; X: float32; Y: float32; Rotation: float32 }

type PhysicsEngine (gravityX, gravityY) =
    let mutable fixtureLookup = new Dictionary<int, Fixture> ()
    let mutable counter = 0
    let world = new World (new Microsoft.Xna.Framework.Vector2 (gravityX, gravityY))

    do
        CreateFloor world 0.0f (ConvertUnits.ToSimUnits (720.f))

    member this.CreateObject width height x y =
        let fixture = CreateDynamicFixture world width height x y
        let physicsObject = { Id = counter; X = fixture.Body.Position.X; Y = fixture.Body.Position.Y; Rotation = fixture.Body.Rotation }
        
        counter <- counter + 1
        fixtureLookup.Add (physicsObject.Id, fixture)
        physicsObject
        
        
    member this.GetUpdatedObject (physicsObject: PhysicsObject) =
        let mutable fixture : Fixture = null
        fixtureLookup.TryGetValue (physicsObject.Id, &fixture) |> ignore
        { physicsObject with X = fixture.Body.Position.X; Y = fixture.Body.Position.Y; Rotation = fixture.Body.Rotation }
        
        
    member this.Tick (timeStep) =
        world.Step timeStep
    