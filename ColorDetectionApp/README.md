# Bright Light Color Detection Application

A C# console application that uses image processing to detect bright lights in images and creates image maps based on the detected points.

## Features

- **Bright Light Detection**: Automatically identifies bright light sources in images
- **Color Analysis**: Uses HSV-based color detection to find high-brightness, low-saturation regions
- **Image Map Generation**: Creates two types of output:
  - **Detection Overlay**: Original image with bounding boxes (red), center points (blue)
  - **Point Map**: Black background with detected light positions marked (cyan)
- **Detailed Analysis**: Provides position, area, and bounding box information for each detected light

## Technical Details

- Built with .NET 8.0
- Uses SixLabors.ImageSharp for cross-platform image processing
- Pure managed code - no native dependencies required
- Detection algorithm:
  - Brightness threshold: 200 (0-255 scale)
  - Saturation threshold: < 100 (to detect white/bright lights)
  - Minimum region size: 50 pixels (to filter noise)
  - Uses flood-fill algorithm for region detection

## Usage

### Basic Usage

```bash
dotnet run <image_path>
```

### Run with Sample Image

```bash
dotnet run
```

When run without arguments, the application generates a sample image with bright spots and processes it automatically.

### Example Output

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

2. **`<input>_point_map.png`**: Black canvas with detected point locations
   - Cyan circles marking the center of each detected light
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
- SixLabors.ImageSharp 3.1.12
- SixLabors.ImageSharp.Drawing 2.1.7
- System.Drawing.Common 10.0.2

## Use Cases

- Light source detection in photographs
- Analyzing stage/studio lighting setups
- Detecting reflections or glare in images
- Mapping LED/bulb positions
- Quality control for lighting installations
- Astronomy image processing (star detection)

## Algorithm Details

The detection works in three phases:

1. **Scanning**: Iterates through all pixels looking for high brightness (RGB max > 200) and low saturation (RGB range < 100)
2. **Region Growing**: Uses flood-fill to find connected bright pixels forming coherent regions
3. **Filtering**: Removes small regions (< 50 pixels) that are likely noise

Each detected region provides:
- Center position (centroid)
- Total pixel count (area)
- Bounding box coordinates

## Customization

You can adjust detection parameters in `Program.cs`:

```csharp
int brightnessThreshold = 200;  // Minimum brightness (0-255)
int saturationThreshold = 100;  // Maximum saturation difference
int minimumArea = 50;           // Minimum pixels to be considered a region
```

## License

This project is open source and available for educational and commercial use.
