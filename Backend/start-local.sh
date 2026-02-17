#!/bin/bash

# Quick Start Script for Local Development (macOS/Linux)

echo ""
echo "======================================"
echo "  Inventory Management Software"
echo "  Local Development Setup"
echo "======================================"
echo ""

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "[ERROR] Docker is not installed"
    echo "Please install Docker Desktop: https://www.docker.com/products/docker-desktop"
    exit 1
fi

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "[ERROR] .NET SDK is not installed"
    echo "Please install .NET 10 SDK: https://dotnet.microsoft.com/download/dotnet/10.0"
    exit 1
fi

# Check if Node.js is installed
if ! command -v node &> /dev/null; then
    echo "[ERROR] Node.js is not installed"
    echo "Please install Node.js: https://nodejs.org/"
    exit 1
fi

echo "[OK] All prerequisites found:"
docker --version
dotnet --version
node --version
echo ""

# Start PostgreSQL
echo "[1/4] Starting PostgreSQL..."
docker-compose up -d
if [ $? -ne 0 ]; then
    echo "[ERROR] Failed to start PostgreSQL"
    exit 1
fi
sleep 3

# Start Backend
echo "[2/4] Starting Backend (.NET API)..."
cd InventoryManagementSoftware.api
dotnet restore > /dev/null 2>&1
echo "Running migrations..."
dotnet ef database update --configuration Local > /dev/null 2>&1

# Open backend in new terminal
if command -v gnome-terminal &> /dev/null; then
    gnome-terminal -- bash -c "cd '$(pwd)' && dotnet run --configuration Local"
elif command -v xterm &> /dev/null; then
    xterm -e "cd '$(pwd)' && dotnet run --configuration Local" &
else
    echo "Starting backend (can't auto-open terminal on this system)..."
    dotnet run --configuration Local &
fi
cd ..
sleep 3

# Start Frontend
echo "[3/4] Starting Frontend (React)..."
cd Frontend
npm install > /dev/null 2>&1

# Open frontend in new terminal
if command -v gnome-terminal &> /dev/null; then
    gnome-terminal -- bash -c "cd '$(pwd)' && npm run dev"
elif command -v xterm &> /dev/null; then
    xterm -e "cd '$(pwd)' && npm run dev" &
elif [[ "$OSTYPE" == "darwin"* ]]; then
    open -a Terminal "$(pwd)"
    cd ..
    echo ""
    echo "======================================"
    echo "  All Services Started!"
    echo "======================================"
    echo ""
    echo "[Backend API]   http://localhost:5000"
    echo "[Frontend]      http://localhost:5173"
    echo "[Database]      localhost:5432"
    echo ""
    echo "In the new Terminal window, run: npm run dev"
    echo ""
    exit 0
else
    echo "Starting frontend (can't auto-open terminal on this system)..."
    npm run dev &
fi
cd ..

echo ""
echo "======================================"
echo "  All Services Started!"
echo "======================================"
echo ""
echo "[Backend API]   http://localhost:5000"
echo "[Frontend]      http://localhost:5173"
echo "[Database]      localhost:5432"
echo ""
echo "Open your browser to http://localhost:5173"
echo ""
echo "To stop all services:"
echo "  - Press Ctrl+C in each terminal"
echo "  - Run: docker-compose down"
echo ""
