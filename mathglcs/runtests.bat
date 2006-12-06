@echo off

rem This runs make with DEBUGONLY set to true to build debug version
rem then compiles testmathgl against the mathgld.dll debug assembly created
rem and runs it
rem
rem The tests are defined by testmathgl.cs

set DEBUGONLY=true
call "%~dp0make"
if errorlevel 1 goto :eof

set COMPILE="%SYSTEMROOT%\Microsoft.Net\Framework\v1.1.4322\csc"
"%COMPILE%" "%~dp0testmathgl.cs" /r:"%~dp0mathgld.dll"
if errorlevel 1 goto :eof

"%~dp0testmathgl"

:eof
