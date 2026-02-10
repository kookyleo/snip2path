@echo off
setlocal

set PROJECT=src\Snip2Path\Snip2Path.csproj

:: Clean
if exist dist rmdir /s /q dist
mkdir dist

echo === Building self-contained (with .NET runtime) ===
dotnet publish %PROJECT% -c Release -r win-x64 --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:IncludeNativeLibrariesForSelfExtract=true ^
  -p:EnableCompressionInSingleFile=true ^
  -o dist\self-contained

echo === Building framework-dependent (requires .NET 8) ===
dotnet publish %PROJECT% -c Release -r win-x64 --self-contained false ^
  -p:PublishSingleFile=true ^
  -o dist\framework-dependent

echo === Packaging zips ===
pushd dist\self-contained
tar -acf ..\Snip2Path-win-x64.zip Snip2Path.exe
popd

pushd dist\framework-dependent
tar -acf ..\Snip2Path-win-x64-compact.zip Snip2Path.exe
popd

echo.
echo Done! Output in dist\
dir dist\*.zip
