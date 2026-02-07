# Bright Light Color Detection Application

A C# console application that uses real-time camera tracking or image processing to detect bright lights and creates image maps based on the detected points.

## Features

### Real-time Camera Mode (New!)
- **Live Brightest Point Tracking**: Automatically finds and tracks the single brightest point in each camera frame
- **Calibration Mode**: Press 'b' to calibrate and capture the baseline color of the brightest point
- **Color Detection**: Displays the RGB color values of the brightest point in real-time
- **Color Difference Analysis**: When calibrated, shows the color difference from the baseline (useful for detecting color changes when light passes through colored objects/boxes)
- **Accumulated Trail Visualization**: Displays all brightest points from previous frames as a trail
- **Connected Path Lines**: Lines connect consecutive tracked points to show the path of movement
- **Interactive Controls**: 
  - Press 'q' to quit
  - Press 'c' to clear the tracked points trail
  - Press 'b' to calibrate with the current brightest point's color
- **Real-time Overlay**: Shows current camera feed with overlaid tracking information
- **Frame Statistics**: Displays count of tracked points and calibration status

### Static Image Mode
- **Bright Light Detection**: Automatically identifies bright light sources in images
- **Color Analysis**: Uses HSV-based color detection to find high-brightness, low-saturation regions
- **Connected Path Visualization**: Lines connect consecutive detected points to show spatial relationships
- **Image Map Generation**: Creates two types of output:
  - **Detection Overlay**: Original image with bounding boxes (red), center points (blue), and connecting lines (green)
  - **Point Map**: Black background with detected light positions marked (cyan) and connecting lines (yellow)
- **Detailed Analysis**: Provides position, area, and bounding box information for each detected light

## Technical Details

- Built with .NET 8.0
- Uses OpenCvSharp4 for real-time camera capture and processing
- Uses SixLabors.ImageSharp for static image processing
- Pure managed code with native OpenCV bindings
- Real-time tracking algorithm:
  - Converts each frame to grayscale
  - Finds pixel with maximum brightness value
  - Tracks location if brightness >= 200 (0-255 scale)
  - Accumulates all points across frames
- Static detection algorithm:
  - Brightness threshold: 200 (0-255 scale)
  - Saturation threshold: < 100 (to detect white/bright lights)
  - Minimum region size: 50 pixels (to filter noise)
  - Uses flood-fill algorithm for region detection

## Usage

### Real-time Camera Mode

```bash
dotnet run
```

When run without arguments, the application opens your default camera and tracks the brightest point in real-time.

**Controls:**
- **'q' or ESC**: Quit the application
- **'c'**: Clear all tracked points and start fresh
- **'b'**: Calibrate using the current brightest point's color as baseline

**What you'll see:**
- Live camera feed
- Current brightest point: Large green circle with outline
- Historical brightest points: Smaller cyan dots forming a trail
- Orange lines connecting consecutive points showing the movement path
- Color information: RGB values of the current brightest point
- Color difference: When calibrated, shows how much the color has changed from baseline
- Status text: "Points tracked: X" showing total count and "[CALIBRATED]" status

### Static Image Mode

```bash
dotnet run <image_path>
```

Processes a static image file and generates output images with detected bright regions.

### Example Output - Real-time Camera Mode

```
Bright Light Color Detection and Mapping
========================================

Starting real-time camera tracking...
Press 'q' to quit, 'c' to clear tracked points

Camera opened successfully!
Resolution: 640x480
Press 'b' to calibrate with brightest point color

[Real-time window displays with overlaid points and connecting lines]

Calibrated! Baseline color: B=245 G=248 R=250

Total points tracked: 543
Camera tracking stopped.
```

### Example Output - Static Image Mode

```
Bright Light Color Detection and Mapping
========================================

Processing image: sample_input.png
Image size: 640x480
Found 4 bright light region(s)
  Light #1:
    Position: (499, 99)
    Area: 1296 pixels
    Bounding Box: (482, 82) - 35x35
  Light #2:
    Position: (399, 199)
    Area: 676 pixels
    Bounding Box: (387, 187) - 25x25
...

Output files created:
  Detection overlay: ./sample_input_detected.png
  Point map: ./sample_input_point_map.png

Processing complete!
```

## Output Files

The application generates two output images:

1. **`<input>_detected.png`**: Original image with detection annotations
   - Red bounding boxes around each bright region
   - Blue center points marking the centroid of each region
   - Green lines connecting consecutive points

2. **`<input>_point_map.png`**: Black canvas with detected point locations
   - Cyan circles marking the center of each detected light
   - Yellow lines connecting consecutive points
   - Useful for spatial analysis and mapping

## Building

```bash
dotnet build
```

## Running

```bash
dotnet run [image_path]
```

## Dependencies

- .NET 8.0 SDK
- OpenCvSharp4 4.10.0 (for real-time camera tracking)
- Platform-specific OpenCV runtime packages (automatically selected based on your OS):
  - **Linux**: OpenCvSharp4.runtime.linux-x64
  - **Windows**: OpenCvSharp4.runtime.win
  - **macOS**: OpenCvSharp4.runtime.osx-x64
- SixLabors.ImageSharp 3.1.12 (for static image processing)
- SixLabors.ImageSharp.Drawing 2.1.7

**Note**: The OpenCV runtime packages are automatically selected based on your platform when you run `dotnet restore` or `dotnet build`. The project file uses MSBuild conditions to ensure the correct runtime is installed for your operating system.

## Use Cases

### Real-time Camera Mode
- Tracking light sources in motion with color change detection
- Calibrating baseline light color for comparison
- Detecting when light passes through colored objects or filters
- Interactive light painting applications
- Laser pointer tracking with path visualization
- Stage light position mapping during performances
- Real-time quality control for lighting installations
- Interactive installations and art projects
- Educational demonstrations of light tracking and color analysis

### Static Image Mode
- Light source detection in photographs
- Analyzing stage/studio lighting setups
- Detecting reflections or glare in images
- Mapping LED/bulb positions
- Quality control for lighting installations
- Astronomy image processing (star detection)

## Algorithm Details

### Real-time Camera Tracking

The real-time tracking works in a simple, efficient manner:

1. **Frame Capture**: Captures frames from the camera at the camera's native framerate
2. **Grayscale Conversion**: Converts the color frame to grayscale for efficient brightness analysis
3. **Brightest Point Detection**: Uses OpenCV's MinMaxLoc to find the pixel with maximum brightness
4. **Color Extraction**: Captures the BGR color values at the brightest point location
5. **Calibration Support**: Press 'b' to store the current color as a baseline reference
6. **Color Difference Calculation**: When calibrated, calculates Euclidean distance in BGR color space between current and baseline colors
7. **Threshold Filtering**: Only tracks points with brightness >= 200 (0-255 scale) to avoid noise
8. **Accumulation**: Stores all brightest points from each frame in a list
9. **Path Visualization**: 
   - Current brightest point: Large green filled circle (8px radius) with outline (10px radius)
   - Historical points: Small cyan filled circles (3px radius)
   - Connecting lines: Orange lines (2px width) between consecutive points
   - Text overlays: Shows RGB color values, color difference (if calibrated), and total count of tracked points

### Static Image Detection

The detection works in three phases:

1. **Scanning**: Iterates through all pixels looking for high brightness (RGB max > 200) and low saturation (RGB range < 100)
2. **Region Growing**: Uses flood-fill to find connected bright pixels forming coherent regions
3. **Filtering**: Removes small regions (< 50 pixels) that are likely noise
4. **Path Connection**: Draws lines connecting consecutive detected points in order of detection

Each detected region provides:
- Center position (centroid)
- Total pixel count (area)
- Bounding box coordinates

Visual outputs include:
- Detection overlay: Red bounding boxes, blue center points, green connecting lines
- Point map: Cyan point markers, yellow connecting lines

## Customization

You can adjust detection parameters in `Program.cs`:

```csharp
int brightnessThreshold = 200;  // Minimum brightness (0-255)
int saturationThreshold = 100;  // Maximum saturation difference
int minimumArea = 50;           // Minimum pixels to be considered a region
```

## License

This project is open source and available for educational and commercial use.
