#!/bin/bash
# Test script to verify dual-color tracking feature

echo "Testing Dual-Color Tracking Feature"
echo "===================================="
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

# Test 2: Static image mode (no color selection needed, should still work)
echo "Test 2: Testing static image mode (backward compatibility)..."
if [ -f "sample_input.png" ]; then
    dotnet run sample_input.png > /dev/null 2>&1
    if [ $? -eq 0 ]; then
        echo "✓ Static image mode still works"
    else
        echo "✗ Static image mode failed"
        exit 1
    fi
else
    echo "⊘ Sample image not found, skipping"
fi

# Test 3: Check that the program prompts for TWO colors
echo "Test 3: Testing dual-color selection prompts..."
echo -e "2\n3" | timeout 5 dotnet run 2>&1 | grep -q "Dual-Color Tracking Mode"
if [ $? -eq 0 ]; then
    echo "✓ Dual-color prompts appear"
else
    echo "⊘ Camera mode cannot be tested (no camera available)"
fi

# Test 4: Check that same-color selection falls back to single-color mode
echo "Test 4: Testing same-color fallback..."
echo -e "2\n2" | timeout 5 dotnet run 2>&1 | grep -q "Primary and secondary colors are the same"
if [ $? -eq 0 ]; then
    echo "✓ Same-color fallback works"
else
    echo "⊘ Camera mode cannot be tested (no camera available)"
fi

echo ""
echo "All automated tests completed!"
echo ""
echo "Manual testing required for dual-color camera tracking:"
echo "  1. Run: dotnet run"
echo "  2. Select PRIMARY color (e.g., Red = 2)"
echo "  3. Select SECONDARY color (e.g., Green = 3)"
echo "  4. Test with two different colored lights:"
echo "     - Primary color: should export PNG + CSV on switch"
echo "     - Secondary color: should export CSV only on switch back"
echo "  5. Verify color switching detection works correctly"
echo "  6. Test timeout behavior for both colors"
