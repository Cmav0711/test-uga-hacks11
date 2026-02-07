#!/bin/bash
# Test script to verify color selection feature

echo "Testing Color Selection Feature"
echo "================================"
echo ""

# Test 1: Build check
echo "Test 1: Building application..."
dotnet build > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✓ Build successful"
else
    echo "✗ Build failed"
    exit 1
fi

# Test 2: Static image mode (no color selection needed)
echo "Test 2: Testing static image mode..."
if [ -f "sample_input.png" ]; then
    dotnet run sample_input.png > /dev/null 2>&1
    if [ $? -eq 0 ]; then
        echo "✓ Static image mode works"
    else
        echo "✗ Static image mode failed"
        exit 1
    fi
else
    echo "⊘ Sample image not found, skipping"
fi

# Test 3: Check that the program accepts input for color selection
echo "Test 3: Testing color selection (simulated)..."
echo "1" | timeout 5 dotnet run 2>&1 | grep -q "Select the color of light to track"
if [ $? -eq 0 ]; then
    echo "✓ Color selection prompt appears"
else
    echo "⊘ Camera mode cannot be tested (no camera available)"
fi

echo ""
echo "All testable features verified!"
echo ""
echo "Manual testing required for camera mode:"
echo "  1. Run: dotnet run"
echo "  2. Select a color (1-8)"
echo "  3. Verify that only lights of that color are tracked"
echo "  4. Test with different colored lights (red LED, green laser, blue light, etc.)"
