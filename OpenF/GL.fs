#nowarn "9" // Disable warning for native interopability

namespace OpenF.GL

open System
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop

module internal NativeGL =
    [<DllImport ("opengl32.dll")>]
    extern void glGenTextures (int n, uint32 *textures)
    
    [<DllImport ("opengl32.dll")>]
    extern void glBindTexture (uint32 target, uint32 texture)
    
    [<DllImport ("opengl32.dll")>]
    extern void glTexImage2D (uint32 target, int level, int internalFormat, int width, int height, int border, uint32 format, uint32 type_, void *data)
    
    [<DllImport ("opengl32.dll")>]
    extern void glTexParameterf (uint32 target, uint32 pname, float32 param)
    
    [<DllImport ("opengl32.dll")>]
    extern void glTexParameteri (uint32 target, uint32 pname, int param)
    
    [<DllImport ("opengl32.dll")>]
    extern void glMatrixMode (uint32 mode)
    
    [<DllImport ("opengl32.dll")>]
    extern void glPushMatrix ()
    
    [<DllImport ("opengl32.dll")>]
    extern void glPopMatrix ()
    
    [<DllImport ("opengl32.dll")>]
    extern void glLoadIdentity ()
    
    [<DllImport ("opengl32.dll")>]
    extern void glTranslated (double x, double y, double z)
    
    [<DllImport ("opengl32.dll")>]
    extern void glTranslatef (float32 x, float32 y, float32 z)
    
    [<DllImport ("opengl32.dll")>]
    extern void glBegin (uint32 mode)
    
    [<DllImport ("opengl32.dll")>]
    extern void glEnd ()   

    [<DllImport ("opengl32.dll")>]
    extern void glTexCoord2f (float32 s, float32 t)
    
    [<DllImport ("opengl32.dll")>]
    extern void glVertex2f (float32 x, float32 y)
    
    [<DllImport ("opengl32.dll")>]
    extern void glRotatef (float32 angle, float32 x, float32 y, float32 z)  
    
    [<DllImport ("opengl32.dll")>]
    extern void glEnable (uint32 cap)
    
    [<DllImport ("opengl32.dll")>]
    extern void glDisable (uint32 cap)
    
    [<DllImport ("opengl32.dll")>]
    extern void glClearColor (float32 red, float32 green, float32 blue, float32 alpha)
    
    [<DllImport ("opengl32.dll")>]
    extern void glOrtho (double left, double right, double bottom, double top, double nearVal, double farVal)
    
    [<DllImport ("opengl32.dll")>]
    extern void glViewport (int x, int y, int width, int height)
    
    [<DllImport ("opengl32.dll")>]
    extern void glClear (uint32 mask)
    
    [<DllImport ("opengl32.dll")>]
    extern uint32 glGetError ()


module GL = 
    let inline private CheckError () =
#if DISABLE_GL_ERROR_CHECKING
        ()
#else
        match enum<Error>(int (NativeGL.glGetError ())) with
        | Error.NoError -> ()
        | Error.InvalidEnum -> raise (Exception "Invalid enum.")
        | Error.InvalidValue -> raise (Exception "Invalid value.")
        | Error.InvalidOperation -> raise (Exception "Invalid operation.")
        | Error.InvalidFramebufferOperation -> raise (Exception "Invalid framebuffer operation.")
        | Error.OutOfMemory -> raise (Exception "Out of memory.")
        | Error.StackUnderflow -> raise (Exception "Stack underflow.")
        | Error.StackOverflow -> raise (Exception "Stack overflow.")
        | _ -> raise (Exception "Invalid GL error.")
#endif
        
    /// <summary>
    /// glGenTextures
    /// </summary>
    let GenerateTextures amount =
        let mutable nativeTextures = NativePtr.stackalloc<uint32> amount
        let textures : uint32[] = Array.zeroCreate amount
        
        NativeGL.glGenTextures (amount, nativeTextures);
        CheckError ()
        
        let source = NativePtr.toNativeInt nativeTextures
        let destination = textures :> obj :?> int[]
        Marshal.Copy (source, destination, 0, amount)
        textures

    /// <summary>
    /// glGenTextures
    /// </summary>  
    let GenerateTexture () =
        (GenerateTextures 1).[0]
        
    /// <summary>
    /// glBindTexture
    /// </summary>
    let BindTexture (target: BindTextureTarget) tid =
        NativeGL.glBindTexture (uint32 target, tid)
        CheckError ()
        
    /// <summary>
    /// glTexImage2D
    /// </summary>
    let TextureImage2D (target: TextureImage2DTarget) level (internalFormat: BaseInternalFormat) width height border (pixelFormat: PixelFormat) (pixelType: PixelType) (data: obj) =
        NativeGL.glTexImage2D (uint32 target, level, int internalFormat, width, height, border, uint32 pixelFormat, uint32 pixelType, data :?> nativeint)
        CheckError ()
      
    /// <summary>
    /// glTexParameterf
    /// </summary>  
    let TextureParameterFloat (target: TextureParameterTarget) (name: TextureParameterName) param =
        NativeGL.glTexParameterf (uint32 target, uint32 name, param)
        CheckError ()
        
    /// <summary>
    /// glTexParameteri
    /// </summary>
    let TextureParameterInt (target: TextureParameterTarget) (name: TextureParameterName) param =
        NativeGL.glTexParameteri (uint32 target, uint32 name, param)
        CheckError ()
     
    /// <summary>
    /// glMatrixMode
    /// </summary>   
    let MatrixMode (mode: MatrixMode) =
        NativeGL.glMatrixMode (uint32 mode)
        CheckError ()
        
    /// <summary>
    /// glPushMatrix
    /// </summary>   
    let PushMatrix () =
        NativeGL.glPushMatrix ()
        CheckError ()
        
    /// <summary>
    /// glPopMatrix
    /// </summary>   
    let PopMatrix () =
        NativeGL.glPopMatrix ()
        CheckError ()
        
    /// <summary>
    /// glMatrixMode
    /// </summary> 
    let LoadIdentity () =
        NativeGL.glLoadIdentity ()
        CheckError ()
        
    /// <summary>
    /// glTranslated
    /// </summary> 
    let TranslateDouble x y z =
        NativeGL.glTranslated (x, y, z)
        CheckError ()
        
    /// <summary>
    /// glTranslatef
    /// </summary> 
    let TranslateFloat x y z =
        NativeGL.glTranslatef (x, y, z)
        CheckError ()
        
    /// <summary>
    /// glBegin
    /// </summary> 
    let Begin (mode: BeginMode) =
        NativeGL.glBegin (uint32 mode)
        
    /// <summary>
    /// glEnd
    /// </summary> 
    let End () =
        NativeGL.glEnd ()
        CheckError ()
        
    /// <summary>
    /// glTexCoord2f
    /// </summary>
    let TextureCoordinate2Float s t =
        NativeGL.glTexCoord2f (s, t)
        
    /// <summary>
    /// glVertex2f
    /// </summary>
    let Vertex2Float x y =
        NativeGL.glVertex2f (x, y)
        
    /// <summary>
    /// glRotatef
    /// </summary>   
    let RotateFloat angle x y z =
        NativeGL.glRotatef (angle, x, y, z)    
        
    /// <summary>
    /// glEnable
    /// </summary>
    let Enable (cap: EnableCap) =
        NativeGL.glEnable (uint32 cap)
        CheckError ()
        
    /// <summary>
    /// glDisable
    /// </summary>
    let Disable (cap: DisableCap) =
        NativeGL.glDisable (uint32 cap)
        CheckError ()
        
    /// <summary>
    /// glClearColor
    /// </summary>
    let ClearColor red green blue alpha =
        NativeGL.glClearColor (red, green, blue, alpha)
        CheckError ()
        
    /// <summary>
    /// glOrtho
    /// </summary>
    let Orthographic left right bottom top near far =
        NativeGL.glOrtho (left, right, bottom, top, near, far)
        CheckError ()
       
    /// <summary>
    /// glViewport
    /// </summary> 
    let Viewport x y width height =
        NativeGL.glViewport (x, y, width, height)
        CheckError ()
        
    /// <summary>
    /// glClear
    /// </summary> 
    let Clear (mask: ClearMask) =
        NativeGL.glClear (uint32 mask)
        CheckError ()

