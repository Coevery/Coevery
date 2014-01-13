@echo off
pushd "%~dp0"
set IisExpress_exe="C:\Program Files (x86)\IIS Express\IisExpress.exe"
if exist %IisExpress_exe% goto FoundIisExpress
set IisExpress_exe="C:\Program Files\IIS Express\IisExpress.exe"
if exist %IisExpress_exe% goto FoundIisExpress
echo IisExpress.exe not found
exit 0

:FoundIisExpress
cd ..\..\build\Orchard14
%IisExpress_exe% /path:%cd% /port:4747 > NUL
popd

