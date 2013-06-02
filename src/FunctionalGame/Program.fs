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
    
    GL.LoadIdentity ()
    GL.BindTexture BindTextureTarget.Texture2D tid
    GL.TranslateFloat (x / 2.f)  (y / 2.f) 0.f
    GL.RotateFloat rotation 0.f 0.f 1.f
    
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
    let width = ConvertUnits.ToSimUnits(16)
    let height = ConvertUnits.ToSimUnits(16)
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

type GameState = { Entities: Entity list; EntitySpawnTime: int64 }

let TimeLoop<'State> state (execution: 'State * Stopwatch -> 'State * bool) =
    let time = Stopwatch.StartNew ()
    let rec TimeLoopRec state =
        match execution (state, time) with
        | (newState, true) -> TimeLoopRec newState
        | _ -> ()
    TimeLoopRec state


[<EntryPoint>]
let main args = 
    GLFW.Init ()
    let wid = GLFW.CreateWindow 1280 720 "Functional Game"
    GLFW.MakeContextCurrent wid
    
    GL.Enable EnableCap.Texture2D
    GL.ClearColor 0.f 1.f 0.f 1.f
    
    GL.MatrixMode MatrixMode.Projection
    GL.LoadIdentity ()
    GL.Orthographic 0. (double 1280) (double 720) 0. 0. 1.
    GL.MatrixMode MatrixMode.ModelView
    
    GL.Disable DisableCap.DepthTest
    
    let entities: Entity list = list.Empty
    
    let world = new World (new Microsoft.Xna.Framework.Vector2 (0.f, 9.82f))
        
    let SpawnEntity entities =
        let fixture = CreateDynamicFixture world 15.f 0.f
        let entity = { Fixture = fixture; }
        entities @ [entity]
    
    let tid = LoadBlock ()
    let floorY = ConvertUnits.ToSimUnits (720.f)
    CreateFloor world 0.0f floorY
    
    TimeLoop { Entities = entities; EntitySpawnTime = int64 0 } (fun (state, time) ->
        match not (GLFW.WindowShouldClose wid) with
        | false -> (state, false)
        | _ ->  
        
        let startTime = time.ElapsedMilliseconds

        world.Step (1.f / 60.f)
        
        GL.Clear (ClearMask.ColorBufferBit)
        
        List.iter (fun entity ->     
            let x = ConvertUnits.ToDisplayUnits (entity.Fixture.Body.Position.X)
            let y = ConvertUnits.ToDisplayUnits (entity.Fixture.Body.Position.Y)
            let rotation = entity.Fixture.Body.Rotation
            RenderBlock 1280 720 x y rotation tid
        ) state.Entities
        
        GLFW.SwapBuffers wid
        GLFW.PollEvents ()
        
        let endTime = time.ElapsedMilliseconds
        let processTime = (1.f / 60.f * 1000.f) - float32 (endTime - startTime)
        Console.WriteLine (processTime)
        if processTime > 0.f then
            Thread.Sleep (int processTime)
            
        match state.EntitySpawnTime < time.ElapsedMilliseconds with
        | true ->
            ({ Entities = SpawnEntity state.Entities; EntitySpawnTime = time.ElapsedMilliseconds + int64 1000 }, true)
        | _ ->    
        (state, true)
    )
            
    GLFW.Terminate ()
    0

