# Real-Time Shape Detection

## Overview

The application now features **real-time shape detection** that recognizes shapes AS YOU DRAW them in the camera feed. This provides immediate visual feedback, showing you what shape the system detects while you're still drawing it.

## How It Works

### Detection Pipeline

1. **Point Tracking**: As you draw with a light source, points are tracked in the camera feed
2. **Frame Throttling**: Detection runs every N frames (default: 15) to balance accuracy and performance
3. **Image Generation**: Tracked points are rendered to a temporary image
4. **Shape Analysis**: The Enhanced Shape Detector analyzes geometric properties
5. **Visual Feedback**: Detected shape name, confidence, and contour overlay are displayed in real-time

### Key Components

#### Enhanced Shape Detector Integration
- Uses the existing `EnhancedShapeDetector.DetectShapeFromMat()` method
- Analyzes circularity, vertices, aspect ratio, and convexity
- Returns shape name, confidence score, and contour points

#### Performance Optimization
- **Frame Interval**: Detection runs every N frames (adjustable)
- **Minimum Points**: Requires at least 10 tracked points before attempting detection
- **Threshold Filtering**: Only updates display if confidence > 40%
- **Efficient Mat Creation**: Reuses temporary Mat objects

## Features

### Visual Feedback

1. **Shape Name Display**
   - Shows detected shape name in UPPERCASE
   - Position: Top-left of screen, below tracking info
   - Color: Green text (bright and visible)
   - Example: "Detected Shape: CIRCLE (Confidence: 85%)"

2. **Confidence Score**
   - Displayed as percentage (0-100%)
   - Updates in real-time as shape evolves
   - Higher confidence = more certain detection

3. **Contour Overlay**
   - Green outline drawn on detected shape
   - Shows the exact boundary the detector found
   - Updates as you draw more points

4. **Progress Indicator**
   - When points < 10: Shows "Draw more points for shape detection (N/10)"
   - Helps users know when detection will start

### Interactive Controls

| Key | Action | Default | Range |
|-----|--------|---------|-------|
| `r` or `R` | Toggle real-time detection on/off | ON | - |
| `d` | Detect MORE frequently (faster updates) | Every 15 frames | 5-60 frames |
| `D` | Detect LESS frequently (better performance) | Every 15 frames | 5-60 frames |

### Detection Frequency Examples

| Interval | Times per Second (30 FPS) | Use Case |
|----------|---------------------------|----------|
| 5 frames | ~6 times/sec | Very responsive, may flicker |
| 10 frames | ~3 times/sec | Smooth and responsive |
| 15 frames (default) | ~2 times/sec | Balanced performance |
| 20 frames | ~1.5 times/sec | More stable, less CPU |
| 30 frames | ~1 time/sec | Very stable, delayed feedback |

## Technical Details

### Minimum Requirements
- **Points**: At least 10 tracked points required
- **Confidence Threshold**: 40% minimum to display
- **Frame Rate**: Optimized for 30 FPS camera

### Detection Algorithm
The real-time detector uses the same geometric analysis as the post-capture detector:
- **Circularity**: 4π × area / perimeter² (perfect circle = 1.0)
- **Vertices**: Polygon approximation with configurable epsilon
- **Aspect Ratio**: Width/height from bounding box
- **Convexity**: Contour area / convex hull area

### Supported Shapes
- **Circle**: High circularity (>0.75)
- **Oval/Ellipse**: Good circularity with non-square aspect ratio
- **Triangle**: 3 vertices
- **Square**: 4 vertices with aspect ratio ~1.0
- **Rectangle**: 4 vertices with aspect ratio ≠ 1.0
- **Diamond**: 4 vertices, rotated
- **Pentagon**: 5 vertices
- **Hexagon**: 6 vertices
- **Heptagon**: 7 vertices
- **Octagon**: 8 vertices
- **Star**: 8+ vertices with low convexity (<0.85)
- **Cross/Plus**: 12+ vertices with low convexity (<0.90)

## Usage Examples

### Example 1: Drawing a Circle
1. Start tracking with your light source
2. After 10 points, you'll see: "Draw more points for shape detection (10/10)"
3. Continue drawing in a circular motion
4. Within 0.5 seconds: "Detected Shape: CIRCLE (Confidence: 82%)"
5. Green contour overlay appears around your circle
6. Confidence increases as you complete the circle: "CIRCLE (Confidence: 91%)"

### Example 2: Drawing a Square
1. Track 10+ points drawing a square
2. Detection shows: "Detected Shape: SQUARE (Confidence: 88%)"
3. Even if your square is slightly imperfect or rotated, it's still recognized
4. The green contour shows the detected boundaries

### Example 3: Adjusting Detection Speed
```
# Start with default (15 frames)
Detected Shape: TRIANGLE (Confidence: 87%)

# Press 'd' for faster updates (10 frames)
> Detection frame interval: every 10 frames (3.0 times per second)
# Updates appear more frequently, but may flicker more

# Press 'D' multiple times for slower, more stable detection (25 frames)
> Detection frame interval: every 20 frames (1.5 times per second)
> Detection frame interval: every 25 frames (1.2 times per second)
# Updates are more stable but less frequent
```

## Performance Tips

1. **For Best Accuracy**
   - Draw slowly and deliberately
   - Keep movements smooth
   - Complete the shape fully
   - Use slower detection (press 'D') for more stable results

2. **For Best Responsiveness**
   - Use faster detection (press 'd')
   - Draw at medium speed
   - Be prepared for slight flickering between shapes

3. **For Best Performance**
   - Use longer detection intervals (press 'D' multiple times)
   - Reduce camera resolution if needed
   - Close other applications

## Comparison: Real-Time vs Post-Capture

| Feature | Real-Time Detection | Post-Capture Detection |
|---------|-------------------|----------------------|
| **Timing** | During drawing | After timeout (3+ seconds) |
| **Feedback** | Immediate visual | Saved to CSV file |
| **Accuracy** | Good (confidence > 40%) | Excellent (confidence > 60%) |
| **Use Case** | Interactive drawing | Final shape recording |
| **Performance** | Frame-throttled | Full analysis |
| **Confidence Threshold** | 40% (lower for responsiveness) | 60% (higher for accuracy) |

**Both methods use the same Enhanced Shape Detector algorithm**, but real-time detection:
- Uses a lower confidence threshold (40% vs 60%) for better responsiveness
- Updates every N frames instead of once per drawing
- Provides visual feedback instead of CSV logging

## Troubleshooting

### Issue: No Shape Detected
**Solution**: Draw at least 10 points and ensure confidence > 40%
- Check counter: "Draw more points for shape detection (N/10)"
- Try drawing a clearer, more defined shape
- Increase detection frequency with 'd' key

### Issue: Shape Flickering Between Types
**Solution**: Decrease detection frequency
- Press 'D' to increase frame interval
- Draw more slowly and deliberately
- This is normal for ambiguous shapes (e.g., circle vs oval)

### Issue: Wrong Shape Detected
**Solution**: Complete the shape more clearly
- Draw the full perimeter
- Keep lines smooth and connected
- Try the post-capture detection (waits for timeout) for more accurate results

### Issue: Performance Lag
**Solution**: Reduce detection frequency
- Press 'D' multiple times to detect less often
- Default of 15 frames should work well on most systems
- Consider reducing camera resolution

## Related Documentation

- [ENHANCED_SHAPE_DETECTION.md](ENHANCED_SHAPE_DETECTION.md) - Details on the shape detection algorithm
- [ColorDetectionApp/CONTOUR_APPROXIMATION.md](ColorDetectionApp/CONTOUR_APPROXIMATION.md) - How contour approximation works
- [README.md](README.md) - Full application documentation

## Technical Implementation

The real-time detection is implemented in `Program.cs` with these key additions:

1. **New Helper Method**: `CreateMatFromPoints()` - Generates a Mat from tracked points
2. **Detection Loop**: Runs every N frames in the main camera loop
3. **Visual Overlay**: Uses `Cv2.DrawContours()` to show detected boundaries
4. **State Management**: Tracks current shape, confidence, and contour
5. **Key Bindings**: Added 'r' for toggle and 'd'/'D' for frequency adjustment

The enhanced shape detector in `EnhancedShapeDetector.cs` now has:
- **New Method**: `DetectShapeFromMat()` - Accepts Mat directly instead of file path
- **Returns Contour**: Provides contour points for visual overlay
- **Same Algorithm**: Uses identical geometric analysis as file-based detection
