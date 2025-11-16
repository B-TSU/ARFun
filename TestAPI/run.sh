#!/bin/bash

# Run API Test Console Application

echo "=== Gemma API Test Console ==="
echo ""

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found!"
    echo "Please install .NET 6.0 or later from https://dotnet.microsoft.com/download"
    exit 1
fi

# Check .NET version
echo "Checking .NET version..."
dotnet --version

echo ""
echo "Building project..."
dotnet build

if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi

echo ""
echo "Running tests..."
echo ""

dotnet run

