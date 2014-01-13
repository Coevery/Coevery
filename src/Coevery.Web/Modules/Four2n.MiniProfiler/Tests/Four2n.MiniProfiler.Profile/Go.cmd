@echo off
call "%~dp0\_environment"
cscript "%wcatfiles%\wcat.wsf" -terminate -run -clients localhost -t "%~dp0\Scripts\%1.txt" -f "%~dp0\settings.txt" -s localhost -singleip -o "%~dp0\Logs\%2.xml"