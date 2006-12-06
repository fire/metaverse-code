@echo off
rem by default builds:
rem  mathgl.dll - optimized build
rem  mathgld.dll - debug build
rem
rem set DEBUGONLY to any value to only build mathgld.dll
rem set OPTIMIZEONLY to any value to only build mathgl.dll

set COMPILE="%SYSTEMROOT%\Microsoft.Net\Framework\v1.1.4322\csc"

if not .%DEBUGONLY%==. echo Warning: debugonly environment variable defined.  Building debug library only.
if not .%DEBUGONLY%==. echo Unset debugonly and rerun make.bat to build optimized library

if .%DEBUGONLY%==. echo Building mathgl.dll (optimized library) ... & %COMPILE% /nologo /optimize /out:mathgl.dll /target:library GLMatrix.cs GLVector.cs
if .%OPTIMIZEDONLY%==. echo Building mathgld.dll (debug library) ... & %COMPILE% /nologo /debug /out:mathgld.dll /target:library GLMatrix.cs GLVector.cs

