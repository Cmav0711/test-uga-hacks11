# test-uga-hacks11

## Bright Light Color Detection Application

A C# application using SixLabors.ImageSharp that detects bright lights in images and creates image maps based on detected points.

### Features
- Detects bright light sources using HSV color analysis
- Creates detection overlay with bounding boxes and center points
- Generates point map visualization
- Provides detailed statistics for each detected light

### Quick Start

```bash
cd ColorDetectionApp
dotnet run
```

This will generate a sample image and process it, creating two output images showing detected bright lights.

### Documentation

See [ColorDetectionApp/README.md](ColorDetectionApp/README.md) for detailed usage instructions and technical documentation.

### Sample Output

The application detects bright lights and provides:
- Position coordinates (x, y)
- Area in pixels
- Bounding box dimensions
- Visual overlays with detection markers
- Point map for spatial analysis