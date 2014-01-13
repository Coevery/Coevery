@echo off
call "%~dp0\_environment"
cscript "%wcatfiles%\wcat.wsf" -clients localhost -terminate -update 