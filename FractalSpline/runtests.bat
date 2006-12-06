@echo off

rem sets WITHOPENGL, NODEFAULT and DEBUGONLY to true then calls make, to build the 
rem FractalSplineGlDebug.dll library
rem Next, compiles testfractalspline.cs against this library
rem and runs it if no errors

set WITHOPENGL=true
set NODEFAULT=true
set DEBUGONLY=true

call "%~dp0make"
if errorlevel 1 goto :eof

set COMPILE="%SYSTEMROOT%\Microsoft.Net\Framework\v1.1.4322\csc"
%COMPILE% /debug "%~dp0source\testfractalspline.cs" "%~dp0source\TextureHelper.cs" /r:DevIL.NET.dll /r:Tao.OpenGl.dll /r:Tao.OpenGl.Glu.dll /r:SdlDotNet.dll /r:Tao.Sdl.dll /r:FractalSplineGlDebug.dll
if errorlevel 1 goto :eof

"%~dp0testfractalspline"
