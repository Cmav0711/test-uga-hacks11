# test-uga-hacks11

## Bright Light Color Detection Application

A C# application using OpenCV and SixLabors.ImageSharp that detects bright lights in real-time from a camera feed or processes static images.

### Features
- **Real-time Camera Tracking**: Tracks the brightest point in each camera frame with calibration support
- **Color Detection and Analysis**: Captures and displays RGB color values of bright points
- **Calibration Mode**: Establish baseline color to detect color changes when light passes through objects
- **Path Visualization**: Displays accumulated trail with lines connecting consecutive points
- **Static Image Processing**: Detects bright light sources using HSV color analysis
- Creates detection overlay with bounding boxes, center points, and connecting lines
- Generates point map visualization with connected paths
- Provides detailed statistics for each detected light

### Quick Start

#### Real-time Camera Mode (Default)
```bash
cd ColorDetectionApp
dotnet run
```

This will open your camera and track the brightest point in real-time:
- The current brightest point is shown as a large green circle
- All previously tracked points are shown as smaller cyan dots
- Orange lines connect consecutive points to show the path
- RGB color values are displayed on screen
- Press 'b' to calibrate and capture baseline color for comparison
- Press 'q' to quit
- Press 'c' to clear the tracked points

#### Static Image Mode
```bash
cd ColorDetectionApp
dotnet run <image_path>
```

This will process a static image and generate output files showing detected bright lights.

### Documentation

See [ColorDetectionApp/README.md](ColorDetectionApp/README.md) for detailed usage instructions and technical documentation.

### Sample Output

#### Real-time Camera Mode
- Displays live camera feed with overlay
- Current brightest point highlighted in green
- Trail of all brightest points from previous frames in cyan
- Orange lines connecting consecutive points
- RGB color information displayed on screen
- Color difference from baseline (when calibrated)
- Frame counter showing number of points tracked
- Calibration status indicator

#### Static Image Mode
The application detects bright lights and provides:
- Position coordinates (x, y)
- Area in pixels
- Bounding box dimensions
- Visual overlays with detection markers and connecting lines
- Point map for spatial analysis with path visualization