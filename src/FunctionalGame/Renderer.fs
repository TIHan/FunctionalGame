namespace FunctionalGame.Renderer

open System
open System.IO
open System.Drawing
open OpenF.GL

type Texture = { Id: int; Width: int; Height: int; BufferId: uint32; }

type IRenderer =
    abstract member Init : unit -> unit
    abstract member Clear : unit -> unit
    abstract member LoadTexture : string -> Texture
    abstract member RenderTexture : Texture * float32 * float32 * float32 -> unit

type GL21Renderer () =
    interface IRenderer with 
    
        member this.Init () =
            GL.Enable EnableCap.Texture2D
            GL.Disable DisableCap.DepthTest
            GL.ClearColor 0.f 1.f 0.f 1.f
            
            GL.MatrixMode MatrixMode.Projection
            GL.LoadIdentity ()
            GL.Orthographic 0. (double 1280) (double 720) 0. 0. 1.
            GL.MatrixMode MatrixMode.ModelView
        
           
        member this.Clear () =
            GL.Clear ClearMask.ColorBufferBit   
                

        member this.LoadTexture filename =
            match File.Exists filename with
            | false -> raise (Exception (String.Format ("Unable to find texture, {0}.", filename)))
            | _ ->
                use block = new Bitmap (filename)
                let data = 
                    block.LockBits (
                        new System.Drawing.Rectangle (0, 0, block.Width, block.Height),
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
                { Id = int tid; Width = data.Width; Height = data.Height; BufferId = 0u }
            

        member this.RenderTexture (texture: Texture, x, y, rotation) =
            let width = float32 texture.Width
            let height = float32 texture.Height
            let tid = uint32 texture.Id
            
            let originX = (width / 2.f)
            let originY = (height / 2.f)
            
            GL.LoadIdentity ()
            GL.BindTexture BindTextureTarget.Texture2D tid
            
            GL.TranslateFloat x y 0.f
            
            GL.TranslateFloat originX originY 0.f
            GL.RotateFloat rotation 0.f 0.f 1.f
            GL.TranslateFloat -originX -originY 0.f
            
            GL.Begin BeginMode.Quads
            
            GL.TextureCoordinate2Float 0.f 0.f
            GL.Vertex2Float 0.f 0.f
            
            GL.TextureCoordinate2Float 1.f 0.f
            GL.Vertex2Float width 0.f
            
            GL.TextureCoordinate2Float 1.f 1.f
            GL.Vertex2Float width height
            
            GL.TextureCoordinate2Float 0.f 1.f
            GL.Vertex2Float 0.f height

            GL.End ()
            

type GL33Renderer () =
    
    let vaid = GL.GenerateVertexArray ()
    
    interface IRenderer with 
   
        member this.Init () =
            
            GL.BindVertexArray vaid
            //GL.Enable EnableCap.Texture2D
            //GL.Disable DisableCap.DepthTest
            GL.ClearColor 0.f 1.f 0.f 1.f
            
            //GL.MatrixMode MatrixMode.Projection
            //GL.LoadIdentity ()
//            GL.Orthographic 0. (double 1280) (double 720) 0. 0. 1.
//            GL.MatrixMode MatrixMode.ModelView
        
           
        member this.Clear () =
            GL.Clear ClearMask.ColorBufferBit   
                

        member this.LoadTexture filename =
            match File.Exists filename with
            | false -> raise (Exception (String.Format ("Unable to find texture, {0}.", filename)))
            | _ ->
                use block = new Bitmap (filename)
                let data = 
                    block.LockBits (
                        new System.Drawing.Rectangle (0, 0, block.Width, block.Height),
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
                
                let vertexData : float32[] = [|
                    0.f; 0.f; 0.f; 
                    0.5f; 0.f; 0.f;
                    0.f; 0.5f; 0.f;
                    0.5f; 0.5f; 0.f;
                |]
                let bid = GL.GenerateBuffer ()
                GL.BindBuffer BindBufferTarget.ArrayBuffer bid
                GL.BufferData BufferDataTarget.ArrayBuffer vertexData BufferDataUsage.StaticDraw
                
                { Id = int tid; Width = data.Width; Height = data.Height; BufferId = bid }
            

        member this.RenderTexture (texture: Texture, x, y, rotation) =
            let width = float32 texture.Width
            let height = float32 texture.Height
            let tid = uint32 texture.Id
            
            let originX = (width / 2.f)
            let originY = (height / 2.f)
            
            let vertexSource = @" 
#version 330
 
in  vec3 in_Position;
 
void
main () 
{
    gl_Position = vec4(in_Position.x, in_Position.y, in_Position.z, 1.0);
    return;
}

"

            let fragmentSource = @"
#version 330
 
precision highp float;
 
out vec4 fragColor;
 
void
main () 
{
    fragColor = vec4 (1.0,1.0,1.0,1.0);
    return;
}

"
            
            let vertexShader = GL.CreateShader ShaderType.Vertex
            let fragmentShader = GL.CreateShader ShaderType.Fragment
            
            GL.ShaderSource vertexShader [| vertexSource |]
            GL.ShaderSource fragmentShader [| fragmentSource |]
            
            GL.CompileShader vertexShader
            GL.CompileShader fragmentShader
            
            let program = GL.CreateProgram ()
            
            GL.AttachShader program vertexShader
            GL.AttachShader program fragmentShader
            
            GL.BindAttributeLocation program 0u "in_Position"
            
            GL.LinkProgram program
            GL.UseProgram program
            
            GL.EnableVertexAttributeArray (0u)
            GL.BindBuffer BindBufferTarget.ArrayBuffer texture.BufferId
            GL.VertexAttributePointer 0u 3 VertexAttributePointerType.Float false 0
            GL.DrawArrays DrawArraysMode.TriangleStrip 0 4
            GL.DisableVertexAttributeArray (0u)
            
            ()
