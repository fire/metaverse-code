@echo off
rem by default builds:
rem  FractalSpline.dll - optimized build
rem  FractalSplineDebug.dll - debug build
rem
rem set DEBUGONLY to any value to only build mathgld.dll
rem set OPTIMIZEONLY to any value to only build mathgl.dll
rem
rem Define WITHOPENGL to build FractalSplineGl.dll and FractalSplineGlDebug.dll (which includes RendererOpenGl class)
rem Define NODEFAULT to suppress build of FractalSplineGl.dll

if .%WITHOPENGL%==. echo Not building OpenGL dlls. Define WITHOPENGL and remake to do so.

set OBJECTS=

set OBJECTS=%OBJECTS% source\Color.cs source\Vector2.cs source\Vector3.cs
set OBJECTS=%OBJECTS% source\TextureMapping.cs

set OBJECTS=%OBJECTS% source\IRenderer.cs

set OBJECTS=%OBJECTS% source\CrossSection.cs
set OBJECTS=%OBJECTS% source\ExtrusionPath.cs source\LinearExtrusionPath.cs source\RotationalExtrusionPath.cs

set OBJECTS=%OBJECTS% source\Primitive.cs source\Face.cs
set OBJECTS=%OBJECTS% source\RotationalPrimitive.cs
set OBJECTS=%OBJECTS% source\Tube.cs source\Ring.cs source\Torus.cs
set OBJECTS=%OBJECTS% source\LinearPrimitive.cs
set OBJECTS=%OBJECTS% source\Box.cs source\Prism.cs source\Cylinder.cs

set COMPILE="%SYSTEMROOT%\Microsoft.Net\Framework\v1.1.4322\csc" /nologo 

if .%NODEFAULT%==. call :standardbuilds
if not .%WITHOPENGL%==. call :openglbuilds

goto :eof

:standardbuilds
rem These need mathgl.dll assembly (from mathglcs)
if .%DEBUGONLY%==. echo Building FractalSpline.dll & %COMPILE% /optimize /out:FractalSpline.dll /target:library /r:mathgl.dll %OBJECTS%
if .%OPTIMIZEDONLY%==. echo Building FractalSplineDebug.dll & %COMPILE% /debug /out:FractalSplineDebug.dll /target:library /r:mathgl.dll %OBJECTS%
goto :eof

:openglbuilds
rem These link with Tao.OpenGl.dll assembly, which should be in current directory
rem These need mathgl.dll assembly (from mathglcs)
if .%DEBUGONLY%==. echo Building FractalSplineGl.dll & %COMPILE% /optimize /out:FractalSplineGl.dll /target:library /r:mathgl.dll /r:Tao.OpenGl.dll %OBJECTS% source\RendererOpenGl.cs
if .%OPTIMIZEDONLY%==. echo Building FractalSplineGlDebug.dll & %COMPILE% /debug /out:FractalSplineGlDebug.dll /target:library /r:mathgl.dll /r:Tao.OpenGl.dll %OBJECTS% source\RendererOpenGl.cs
goto :eof
