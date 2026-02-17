@echo off
REM Quick Start Script for Local Development (Windows)

echo.
echo ======================================
echo   Inventory Management Software
echo   Local Development Setup
echo ======================================
echo.

REM Check if Docker is installed
docker --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker is not installed or not in PATH
    echo Please install Docker Desktop: https://www.docker.com/products/docker-desktop
    pause
    exit /b 1
)

REM Check if .NET SDK is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] .NET SDK is not installed or not in PATH
    echo Please install .NET 10 SDK: https://dotnet.microsoft.com/download/dotnet/10.0
    pause
    exit /b 1
)

REM Check if Node.js is installed
node --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Node.js is not installed or not in PATH
    echo Please install Node.js: https://nodejs.org/
    pause
    exit /b 1
)

echo [OK] All prerequisites found:
docker --version
dotnet --version
node --version
echo.

REM Start PostgreSQL
echo [1/4] Starting PostgreSQL...
docker-compose up -d
if errorlevel 1 (
    echo [ERROR] Failed to start PostgreSQL
    pause
    exit /b 1
)
timeout /t 3 /nobreak

REM Start Backend
echo [2/4] Starting Backend (.NET API)...
cd InventoryManagementSoftware.api
dotnet restore >nul 2>&1
echo Running migrations...
dotnet ef database update --configuration Local >nul 2>&1
start "Inventory API" cmd /k "dotnet run --configuration Local"
cd ..
timeout /t 3 /nobreak

REM Start Frontend
echo [3/4] Starting Frontend (React)...
cd Frontend
call npm install >nul 2>&1
start "Inventory Frontend" cmd /k "npm run dev"
cd ..

echo.
echo ======================================
echo   All Services Started!
echo ======================================
echo.
echo [Backend API]   http://localhost:5000
echo [Frontend]      http://localhost:5173
echo [Database]      localhost:5432
echo.
echo Open your browser to http://localhost:5173
echo.
echo To stop all services:
echo   - Close the terminal windows
echo   - Run: docker-compose down
echo.
pause
