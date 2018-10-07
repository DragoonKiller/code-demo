echo off

cd lib
cargo build
set lib_build_suc=%errorlevel%
cd ..
if NOT %lib_build_suc%==0 exit
xcopy /Y .\lib\target\debug\logic.dll .\bin

dotnet build ./main/x.csproj
set main_build_suc=%errorlevel%
echo %main_build_suc%
if NOT %main_build_suc%==0 exit
xcopy /Y .\main\bin\Main.exe .\bin

echo on

.\bin\Main.exe
