#nowarn "9" // Disable warning for native interopability

namespace OpenF.GLFW

open System
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop

// http://www.glfw.org/docs/3.0/quick.html

module internal NativeGLFW =
    [<DllImport ("glfw3.dll")>]
    extern int glfwInit ()
    
    [<DllImport ("glfw3.dll")>]
    extern void glfwTerminate ()
    
    [<DllImport ("glfw3.dll")>]
    extern nativeint glfwCreateWindow (int width, int height, string title, nativeint monitor, nativeint share)
    
    [<DllImport ("glfw3.dll")>]
    extern nativeint glfwGetPrimaryMonitor ()
    
    [<DllImport ("glfw3.dll")>]
    extern void glfwMakeContextCurrent (nativeint window)
    
    [<DllImport ("glfw3.dll")>]
    extern int glfwWindowShouldClose (nativeint window)
    
    [<DllImport ("glfw3.dll")>]
    extern void glfwSwapBuffers (nativeint window)
    
    [<DllImport ("glfw3.dll")>]
    extern void glfwSwapInterval (int interval)
    
    [<DllImport ("glfw3.dll")>]
    extern void glfwPollEvents ()
    
module GLFW =
    let Init () =
        match NativeGLFW.glfwInit () with
        | 0 -> raise (Exception "Unable to initialize GLFW.")
        | _ -> ()
        
    let Terminate () =
        NativeGLFW.glfwTerminate ()
        
    // FIXME
    let CreateWindow width height title monitorHandle =
        match NativeGLFW.glfwCreateWindow (width, height, title, nativeint (int monitorHandle), IntPtr.Zero) with
        | x when x = IntPtr.Zero ->
            Terminate ()
            raise (Exception "Unable to create GLFW window.")
        | x -> int x
        
    let GetPrimaryMonitor () =
        int (NativeGLFW.glfwGetPrimaryMonitor ())
        
    let MakeContextCurrent (windowHandle: int) =
        NativeGLFW.glfwMakeContextCurrent (nativeint windowHandle)
        
    let WindowShouldClose (windowHandle: int) =
        match NativeGLFW.glfwWindowShouldClose (nativeint windowHandle) with
        | 0 -> false
        | _ -> true
        
    let SwapBuffers (windowHandle: int) =
        NativeGLFW.glfwSwapBuffers (nativeint windowHandle)
        
    let SwapInterval interval =
        NativeGLFW.glfwSwapInterval interval
        
    let PollEvents () =
        NativeGLFW.glfwPollEvents ()
    
