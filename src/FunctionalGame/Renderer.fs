module FunctionalGame.Renderer

open System
open System.IO
open System.Drawing
open OpenF.GL

let Init () =
    GL.Enable EnableCap.Texture2D
    GL.Disable DisableCap.DepthTest
    GL.ClearColor 0.f 1.f 0.f 1.f
    
    GL.MatrixMode MatrixMode.Projection
    GL.LoadIdentity ()
    GL.Orthographic 0. (double 1280) (double 720) 0. 0. 1.
    GL.MatrixMode MatrixMode.ModelView   


let LoadBlock () =
    let filename = "Content/Textures/Block/BLOCK.png"
    match File.Exists filename with
    | false -> raise (Exception ("Unable to find texture."))
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
        

let LoadTexture filename =
    match File.Exists filename with
    | false -> raise (Exception (String.Format ("Unable to find texture, {0}.", filename)))
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

            
let RenderBlock x y rotation tid  =
    
    GL.LoadIdentity ()
    GL.BindTexture BindTextureTarget.Texture2D tid
    GL.TranslateFloat x y 0.f
    GL.TranslateFloat 8.0f 8.0f 0.f
    GL.RotateFloat rotation 0.f 0.f 1.f
    GL.TranslateFloat -8.0f -8.0f 0.f
    
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
    

let RenderEntity width height x y rotation tid =
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
