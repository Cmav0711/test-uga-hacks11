# test-uga-hacks11

## Bright Light Color Detection Application

A C# application using OpenCV and SixLabors.ImageSharp that detects bright lights in real-time from a camera feed or processes static images.

### Features
- **Real-time Camera Tracking**: Tracks the brightest point in each camera frame and displays accumulated trail
- **Static Image Processing**: Detects bright light sources using HSV color analysis
- Creates detection overlay with bounding boxes and center points
- Generates point map visualization
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
- Frame counter showing number of points tracked

#### Static Image Mode
The application detects bright lights and provides:
- Position coordinates (x, y)
- Area in pixels
- Bounding box dimensions
- Visual overlays with detection markers
- Point map for spatial analysis