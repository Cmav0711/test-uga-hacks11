# Contour Approximation Feature

## Overview

Contour approximation is a technique used in computer vision to reduce the number of points in a contour by approximating it with a polygon that has fewer vertices while maintaining its essential shape characteristics. This feature provides configurable and interactive contour approximation capabilities for the color detection application.

## What is Contour Approximation?

Contour approximation uses the Douglas-Peucker algorithm (via OpenCV's `ApproxPolyDP` function) to simplify a contour by:
1. Finding points that deviate significantly from a straight line between two endpoints
2. Recursively applying this process to create a simplified polygon
3. Using an epsilon parameter to control the level of approximation

### Epsilon Parameter

The epsilon parameter controls how much detail is preserved:
- **Lower epsilon** (e.g., 0.01 = 1%): More detailed approximation, preserves more points
- **Higher epsilon** (e.g., 0.10 = 10%): More simplified approximation, fewer points
- **Default epsilon**: 0.04 (4% of the perimeter)

The epsilon is typically specified as a factor of the contour's perimeter.

## Features

### 1. Configurable Contour Approximation

The `ContourApproximation` class provides several methods for working with contour approximation:

#### Basic Approximation
```csharp
// Approximate a contour with a custom epsilon factor
Point[] approximated = ContourApproximation.ApproximateContour(contour, 0.04);
```

#### Multi-Level Analysis
```csharp
// Analyze contour at multiple epsilon levels
var results = ContourApproximation.AnalyzeAtMultipleLevels(contour);
// Returns approximation results at 0.5%, 1%, 2%, 4%, 6%, 8%, and 10% epsilon
```

### 2. Enhanced Shape Detector Integration

The `EnhancedShapeDetector` now supports configurable epsilon factors:

```csharp
// Detect shape with custom epsilon
var (shape, confidence) = EnhancedShapeDetector.DetectShape(imagePath, epsilonFactor: 0.02);

// Detect multiple shapes with custom epsilon
var shapes = EnhancedShapeDetector.DetectMultipleShapes(imagePath, epsilonFactor: 0.06);

// Get detailed analysis with custom epsilon
string details = EnhancedShapeDetector.GetShapeAnalysisDetails(imagePath, epsilonFactor: 0.04);
```

## Command-Line Usage

### 1. Contour Approximation Info
Get detailed information about contour approximation for an image:

```bash
dotnet run --contour-info <image_path> [epsilon_factor]
```

**Example:**
```bash
dotnet run --contour-info symbols/star/star_1.png 0.04
```

**Output:**
```
Contour Approximation Analysis:
- Original Points: 247
- Approximated Points: 12
- Compression Ratio: 0.049 (4.9%)
- Epsilon Factor: 0.04 (4.0%)
- Epsilon Value: 15.32 pixels
- Perimeter: 383.2 pixels
- Original Area: 8543.5 pixels²
- Approximated Area: 8512.1 pixels²
- Area Preservation: 99.6%
```

### 2. Visualize Approximation Levels
Generate visualizations at multiple epsilon levels:

```bash
dotnet run --test-contour-approx <image_path>
```

**Example:**
```bash
dotnet run --test-contour-approx symbols/circle/circle_1.png
```

This will:
- Analyze the image at 7 different epsilon levels (0.5%, 1%, 2%, 4%, 6%, 8%, 10%)
- Generate a PNG visualization for each level
- Show the original contour (blue), approximated contour (green), and vertices (red)
- Display statistics for each level

**Generated files:**
- `contour_approx_eps0.005.png`
- `contour_approx_eps0.010.png`
- `contour_approx_eps0.020.png`
- `contour_approx_eps0.040.png`
- `contour_approx_eps0.060.png`
- `contour_approx_eps0.080.png`
- `contour_approx_eps0.100.png`

### 3. Interactive Contour Approximation
Interactively adjust epsilon and see results in real-time:

```bash
dotnet run --interactive-contour <image_path>
```

**Example:**
```bash
dotnet run --interactive-contour symbols/square/square_1.png
```

**Interactive Controls:**
- `+` or `=`: Increase epsilon (less detail, fewer points)
- `-`: Decrease epsilon (more detail, more points)
- `r`: Reset to default epsilon (0.04)
- `s`: Save current visualization to file
- `q` or `ESC`: Quit

The window will display:
- Original contour in blue
- Approximated contour in green
- Vertices as red circles
- Real-time statistics (epsilon, points, compression ratio)

### 4. Shape Analysis with Custom Epsilon
Analyze shapes with a custom epsilon factor:

```bash
dotnet run --analyze-shape <image_path> [epsilon_factor]
```

**Example:**
```bash
dotnet run --analyze-shape symbols/triangle/triangle_1.png 0.02
```

## Use Cases

### 1. Shape Recognition
Different shapes may require different epsilon values for optimal recognition:
- **Circles/Ovals**: Higher epsilon (0.06-0.10) works well since few points are needed
- **Stars/Complex shapes**: Lower epsilon (0.02-0.04) preserves important details
- **Polygons**: Medium epsilon (0.04-0.06) balances detail and simplification

### 2. Performance Optimization
Reduce computational overhead by approximating contours:
- Fewer points = faster processing
- Useful for real-time applications
- Balance between accuracy and speed

### 3. Noise Reduction
Higher epsilon values can help filter out noise in contours:
- Remove small irregularities
- Smooth hand-drawn shapes
- Focus on major shape features

### 4. Data Compression
Reduce the amount of data needed to represent a shape:
- Store fewer points while maintaining shape characteristics
- Useful for transmission or storage
- Area preservation typically > 95% even with significant compression

## Technical Details

### Algorithm: Douglas-Peucker
The contour approximation uses the Douglas-Peucker algorithm, which:
1. Finds the point in the contour that is farthest from the line connecting the endpoints
2. If this distance is greater than epsilon, the contour is split at that point
3. The algorithm recursively processes each segment
4. Points that are within epsilon distance of the approximation are removed

### Metrics

**Compression Ratio**: `approximated_points / original_points`
- Values closer to 0 indicate more compression
- Typical range: 0.02 (98% reduction) to 0.5 (50% reduction)

**Area Preservation**: `(approximated_area / original_area) × 100%`
- Indicates how well the approximation preserves the shape's area
- Typically > 95% even with significant point reduction

**Epsilon as Percentage**: `epsilon_factor × 100`
- Common values: 1% (detailed), 4% (balanced), 10% (simplified)
- Relative to the contour's perimeter

## Examples

### Example 1: Circle Detection
```bash
dotnet run --test-contour-approx symbols/circle/circle_1.png
```
- **Epsilon 0.5%**: 156 points → 45 points (28.8% compression)
- **Epsilon 4%**: 156 points → 8 points (5.1% compression)
- **Epsilon 10%**: 156 points → 6 points (3.8% compression)
- Circle is still recognizable even at 10% epsilon

### Example 2: Star Detection
```bash
dotnet run --test-contour-approx symbols/star/star_1.png
```
- **Epsilon 0.5%**: 247 points → 38 points (15.4% compression)
- **Epsilon 2%**: 247 points → 18 points (7.3% compression)
- **Epsilon 4%**: 247 points → 12 points (4.9% compression)
- Star's concave points preserved at lower epsilon values

### Example 3: Custom Epsilon for Shape Analysis
```bash
# Analyze with default epsilon (4%)
dotnet run --analyze-shape symbols/hexagon/hexagon_1.png

# Analyze with lower epsilon for more detail (1%)
dotnet run --analyze-shape symbols/hexagon/hexagon_1.png 0.01

# Analyze with higher epsilon for simplification (8%)
dotnet run --analyze-shape symbols/hexagon/hexagon_1.png 0.08
```

## Integration with Existing Features

### Shape Detection
The enhanced shape detector automatically uses contour approximation with a default epsilon of 0.04. You can now customize this:

```csharp
// In your code
var (shape, confidence) = EnhancedShapeDetector.DetectShape("path/to/image.png", 0.02);
```

### Symbol Recognition
When symbols are detected in circular screenshots, contour approximation is used to classify them efficiently.

## Best Practices

1. **Start with Default**: The default epsilon (0.04 = 4%) works well for most cases
2. **Experiment Interactively**: Use `--interactive-contour` to find the optimal epsilon for your specific shapes
3. **Consider Shape Complexity**: Complex shapes need lower epsilon, simple shapes can use higher epsilon
4. **Monitor Area Preservation**: Ensure the approximated area is still close to the original (> 90%)
5. **Balance Speed vs Accuracy**: Higher epsilon = faster processing but potentially less accurate

## References

- OpenCV Documentation: [Contour Approximation](https://docs.opencv.org/4.x/dd/d49/tutorial_py_contour_features.html)
- Douglas-Peucker Algorithm: [Wikipedia](https://en.wikipedia.org/wiki/Ramer%E2%80%93Douglas%E2%80%93Peucker_algorithm)
- OpenCV ApproxPolyDP: [API Reference](https://docs.opencv.org/4.x/d3/dc0/group__imgproc__shape.html#ga0012a5fdaea70b8a9970165d98722b4c)
