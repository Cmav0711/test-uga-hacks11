#!/bin/bash
# Test script for camera tracking functionality

echo "Testing ColorDetectionApp Camera Tracking"
echo "=========================================="
echo ""

# Check if we're in the right directory
if [ ! -f "Program.cs" ]; then
    echo "Error: Please run this script from the ColorDetectionApp directory"
    exit 1
fi

echo "1. Building the application..."
dotnet build
if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi
echo "Build successful!"
echo ""

echo "2. Testing static image mode..."
if [ -f "sample_input.png" ]; then
    dotnet run sample_input.png
    if [ $? -ne 0 ]; then
        echo "Static image test failed!"
        exit 1
    fi
    echo "Static image mode works!"
else
    echo "sample_input.png not found, skipping static image test"
fi
echo ""

echo "3. Camera mode instructions:"
echo "   To test camera mode, run: dotnet run"
echo "   - Make sure a camera is connected"
echo "   - A window will open showing the live camera feed"
echo "   - The brightest point in each frame will be marked with a green circle"
echo "   - All previous brightest points will be shown as cyan dots"
echo "   - Press 'q' to quit"
echo "   - Press 'c' to clear all tracked points"
echo ""

echo "All tests completed successfully!"
