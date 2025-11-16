@echo off
REM Run API Test Console Application

echo === Gemma API Test Console ===
echo.

REM Check if .NET is installed
where dotnet >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo ❌ .NET SDK not found!
    echo Please install .NET 6.0 or later from https://dotnet.microsoft.com/download
    exit /b 1
)

REM Check .NET version
echo Checking .NET version...
dotnet --version

echo.
echo Building project...
dotnet build

if %ERRORLEVEL% NEQ 0 (
    echo ❌ Build failed!
    exit /b 1
)

echo.
echo Running tests...
echo.

dotnet run

