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

let LoadBlock () =
    let filename = "block.png"
    match File.Exists filename with
    | false -> raise (Exception ("Unable to find block.png."))
    | _ ->
        use block = new Bitmap (filename)
        let data = 
            block.LockBits (
                new Rectangle (0, 0, block.Width, block.Height),
                Imaging.ImageLockMode.ReadOnly,
                Imaging.PixelFormat.Format32bppArgb
            )
        
        let mutable tid = GL.GenerateTexture ()
        GL.BindTexture BindTextureTarget.Texture2D tid

        GL.TextureImage2D
            TextureImage2DTarget.Texture2D
            0
            BaseInternalFormat.Rgba
            data.Width
            data.Height
            0
            PixelFormat.Bgra
            PixelType.UnsignedByte
            data.Scan0
        
        block.UnlockBits (data)
        GL.TextureParameterInt TextureParameterTarget.Texture2D TextureParameterName.TextureMinFilter (int TextureMinFilter.Linear)
        GL.TextureParameterInt TextureParameterTarget.Texture2D TextureParameterName.TextureMagFilter (int TextureMagFilter.Linear)
        tid

            
let RenderBlock viewportWidth viewportHeight x y rotation tid  =
    let translateX = x / float32 viewportWidth * 2.f
    let translateY = y / float32 viewportHeight * -2.f
    GL.MatrixMode MatrixMode.Projection
    GL.LoadIdentity ()
    GL.TranslateFloat (translateX / 2.f) (translateY / 2.f) 0.f
    GL.BindTexture BindTextureTarget.Texture2D tid
    
    GL.Begin BeginMode.Quads
    
    GL.TextureCoordinate2Float 0.f 0.f
    GL.Vertex2Float 0.f 0.f
    
    GL.TextureCoordinate2Float 1.f 0.f
    GL.Vertex2Float 16.f 0.f
    
    GL.TextureCoordinate2Float 1.f 1.f
    GL.Vertex2Float 16.f 16.f
    
    GL.TextureCoordinate2Float 0.f 1.f
    GL.Vertex2Float 0.f 16.f

    GL.End ()
    
      
let CreateDynamicFixture world x y =
    let position = Nullable<Microsoft.Xna.Framework.Vector2> (new Microsoft.Xna.Framework.Vector2(x, y))
    let body = new Body (world, position)
    let width = ConvertUnits.ToSimUnits(8)
    let height = ConvertUnits.ToSimUnits(8)
    let shape = new Shapes.PolygonShape (PolygonTools.CreateRectangle(width, height), 1.0f)
    let fixture = body.CreateFixture shape
    
    fixture.Restitution <- 0.5f
    fixture.Body.BodyType <- BodyType.Dynamic
    fixture.Body.Mass <- 1.0f
    fixture.Body.SleepingAllowed <- true
    fixture  


let CreateFloor world x y =
    let position = Nullable<Microsoft.Xna.Framework.Vector2> (new Microsoft.Xna.Framework.Vector2 (x, y))
    let body = new Body (world, position)    
    let shape = new Shapes.PolygonShape (PolygonTools.CreateRectangle(1000.0f, 0.1f), 0.0f)
    body.CreateFixture shape |> ignore


type Entity = { Fixture: Fixture; }


[<EntryPoint>]
let main args = 
    GLFW.Init ()
    let wid = GLFW.CreateWindow 1280 720 "Functional Game"
    GLFW.MakeContextCurrent wid
    
    GL.Enable EnableCap.Texutre2D
    GL.ClearColor 0.f 1.f 0.f 1.f
    
    GL.Orthographic 0. (double 1280) (double 720) 0. -1. 1.
    GL.Viewport 0 0 1280 720
    
    let mutable entities = new List<Entity> ()
    let world = new World (new Microsoft.Xna.Framework.Vector2 (0.f, 9.82f))
    
    let SpawnEntity (ents: List<Entity>) =
        let fixture = CreateDynamicFixture world 15.f 0.f
        let entity = { Fixture = fixture; }
        ents.Add entity
    
    let tid = LoadBlock ()
    let floorY = ConvertUnits.ToSimUnits (720.f)
    CreateFloor world 0.0f floorY
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    SpawnEntity entities
    
    
    let gameTime = new Stopwatch ()
    gameTime.Start ()
    
    while not (GLFW.WindowShouldClose wid) do
        let startTime = gameTime.ElapsedMilliseconds   
        //world.Step (1.f / 60.f)
        
        GL.Clear (ClearMask.ColorBufferBit ||| ClearMask.DepthBufferBit)

        entities.ForEach (fun entity ->     
            let x = ConvertUnits.ToDisplayUnits (entity.Fixture.Body.Position.X)
            let y = ConvertUnits.ToDisplayUnits (entity.Fixture.Body.Position.Y)
            let rotation = ConvertUnits.ToDisplayUnits (entity.Fixture.Body.Rotation)
            RenderBlock 1280 720 x y rotation tid
        )
        
        GLFW.SwapBuffers wid
        GLFW.PollEvents ()
        
        let endTime = gameTime.ElapsedMilliseconds
        let processTime = (1.f / 60.f * 1000.f) - float32 (endTime - startTime)
        Console.WriteLine (processTime)
        if processTime > 0.f then
            Thread.Sleep (int processTime)
            
    GLFW.Terminate ()
    0

