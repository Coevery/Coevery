CScript //H:CScript

set wcatfiles=%ProgramFiles%\wcat
if exist "%wcatfiles%\wcat.wsf" goto FoundWcat
set wcatfiles=%ProgramFiles(x86)%\wcat
if exist "%wcatfiles%\wcat.wsf" goto FoundWcat
REM set wcatfiles=%~dp0\..\..\lib\wcat
REM if exist "%wcatfiles%\wcat.wsf" goto FoundWcat
echo Wcat not found

:FoundWcat
