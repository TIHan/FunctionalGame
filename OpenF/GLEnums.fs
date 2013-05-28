namespace OpenF.GL

open System
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop

type internal Error =
    | NoError = 0
    | InvalidEnum = 0x0500
    | InvalidValue = 0x0501
    | InvalidOperation = 0x0502
    | InvalidFramebufferOperation = 0x506
    | OutOfMemory = 0x0505
    | StackUnderflow = 0x0504
    | StackOverflow = 0x0503

type BindTextureTarget = 
    | Texture1D = 0x0DE0
    | Texture2D = 0x0DE1
    | Texture3D = 0x806F
    | Texture1DArray = 0x8C18
    | Texture2DArray = 0x8C1A
    | TextureRectangle = 0x84F5
    | TextureCubeMap = 0x8513
    | TextureCubeMapArray = 0x9009
    | TextureBuffer = 0x8C2A
    | Texture2DMultisample = 0x9100
    | Texture2DMultisampleArray = 0x9102
    
type TextureParameterTarget =
    | Texture1D = 0x0DE0
    | Texture2D = 0x0DE1
    | Texture3D = 0x806F
    | Texture1DArray = 0x8C18
    | Texture2DArray = 0x8C1A
    | TextureRectangle = 0x84F5
    | TextureCubeMap = 0x8513
    
type TextureMagFilter =
    | Nearest = 0x2600
    | Linear = 0x2601
    
// TODO
type TextureMinFilter =
    | Nearest = 0x2600
    | Linear = 0x2601
    
// TODO
type TextureParameterName =
    | TextureMagFilter = 0x2800
    | TextureMinFilter = 0x2801
    
// TODO
type TextureImage2DTarget =
    | Texture2D = 0x0DE1
    
type BaseInternalFormat =
    | DepthComponent = 0x1902
    | DepthStencil = 0x84F9
    | Red = 0x1903
    | Rg = 0x8227
    | Rgb = 0x1907
    | Rgba = 0x1908
    
type PixelFormat =
    | Red = 0x1903
    | Rg = 0x8227
    | Rgb = 0x1907
    | Bgr = 0x80E0
    | Rgba = 0x1908
    | Bgra = 0x80E1
    
// TODO
type PixelType =
    | UnsignedByte = 0x1401
    
type MatrixMode =
    | Modelview = 0x1700
    | Projection = 0x1701
    | Texture = 0x1702
    
// TODO
type BeginMode =
    | Quads = 0x0007
    
// TODO
type EnableCap =
    | Texutre2D = 0x0DE1
    
type ClearMask =
    | ColorBufferBit = 0x00004000
    | StencilBufferBit = 0x00000400
    | DepthBufferBit = 0x00000100
    | AccumBufferBit = 0x00000200
