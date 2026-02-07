# Symbol Detection Usage Guide

This guide demonstrates how to use the symbol detection feature in the Bright Light Color Detection Application.

## Quick Start

### 1. Generate Symbol Templates (First Time Only)

Before using symbol detection, generate the example templates:

```bash
cd ColorDetectionApp
dotnet run --generate-symbols
```

This creates 5 example symbols in the `symbols/` directory:
- `circle.png` - A white circle
- `square.png` - A white square
- `triangle.png` - A white triangle
- `star.png` - A white 5-pointed star
- `cross.png` - A white cross/plus sign

### 2. Run the Application

Start the camera tracking application:

```bash
dotnet run
```

Follow the prompts to select a color for tracking.

### 3. Capture Screenshots

During operation:
- Press **'s'** to capture a circular screenshot
- The screenshot is automatically saved and analyzed for symbols
- Results are appended to `detected_symbols.csv`

Alternatively:
- Draw with a light source
- When light disappears for 3 seconds (configurable), the drawing is auto-exported
- Both PNG and symbol detection happen automatically

## Understanding the Output

### detected_symbols.csv Format

The CSV file contains the following columns:

| Column | Description | Example |
|--------|-------------|---------|
| **Timestamp** | Date and time of detection | `2024-02-07 18:15:23` |
| **ImageFilename** | Name of analyzed image | `circular_screenshot_20240207_181523.png` |
| **DetectedSymbols** | Semicolon-separated symbol names | `circle;star` or `None` |
| **Confidence** | Semicolon-separated confidence scores (0-1) | `0.892;0.754` or `N/A` |
| **Count** | Number of symbols detected | `2` or `0` |

### Example CSV Content

```csv
Timestamp,ImageFilename,DetectedSymbols,Confidence,Count
2024-02-07 18:15:23,circular_screenshot_20240207_181523.png,circle;star,0.892;0.754,2
2024-02-07 18:16:45,light_drawing_20240207_181645.png,square,0.812,1
2024-02-07 18:18:12,circular_screenshot_20240207_181812.png,None,N/A,0
```

## Customizing Symbol Templates

### Adding Your Own Symbols

1. Create a PNG or JPEG image of your symbol
2. Use high contrast (white symbol on black background works best)
3. Keep it reasonably sized (100x100 to 200x200 pixels recommended)
4. Save it in the `symbols/` directory with a descriptive name
5. The filename (without extension) becomes the symbol name in results

Example:
```
symbols/
  ├── heart.png        # Your custom heart symbol
  ├── arrow.png        # Your custom arrow symbol
  └── letter_A.png     # Your custom letter A symbol
```

### Symbol Template Best Practices

For best detection results:
- **High Contrast**: White symbols on black background (or vice versa)
- **Clear Shapes**: Avoid overly complex or detailed symbols
- **Consistent Sizing**: Keep templates similar in size to expected captures
- **Single Symbol**: One symbol per template file
- **Clean Edges**: Sharp, clear boundaries work better than fuzzy edges

## How Symbol Detection Works

The application uses OpenCV's template matching algorithm:

1. **Load Template**: Reads all PNG/JPG files from `symbols/` directory
2. **Convert to Grayscale**: Both captured image and templates are converted to grayscale
3. **Template Matching**: Uses CCoeffNormed matching method
4. **Confidence Threshold**: Only matches with ≥70% confidence are reported
5. **Record Results**: Appends findings to `detected_symbols.csv`

### Confidence Scores

- **0.90 - 1.00**: Excellent match, very high confidence
- **0.80 - 0.89**: Good match, high confidence
- **0.70 - 0.79**: Acceptable match, moderate confidence
- **< 0.70**: Rejected (not reported)

## Testing Symbol Detection

Run the built-in test to verify everything works:

```bash
dotnet run --test-symbols
```

This will:
1. Generate symbol templates (if not already present)
2. Create a test image with a circle
3. Run symbol detection on the test image
4. Report results

## Troubleshooting

### No Symbols Detected

If symbols aren't being detected:
- Ensure `symbols/` directory exists and contains valid image files
- Check that captured images have clear, visible symbols
- Verify symbol templates match the style/appearance of captured symbols
- Consider adjusting the confidence threshold in the code if needed

### OpenCV Initialization Errors

In headless environments (no display), OpenCV may fail to initialize. This is expected and doesn't affect:
- Symbol template generation
- File organization
- CSV file structure

The symbol detection will work properly when running with a camera in a graphical environment.

## Example Workflow

1. **Setup** (one time):
   ```bash
   dotnet run --generate-symbols
   ```

2. **Run application**:
   ```bash
   dotnet run
   ```

3. **During operation**:
   - Select color to track (e.g., option 3 for Green)
   - Point a green light at the camera
   - Move the light to draw shapes (e.g., draw a circle)
   - Press 's' to capture, or wait for auto-export

4. **Review results**:
   ```bash
   cat detected_symbols.csv
   ```

5. **See captured images**:
   ```bash
   ls -lh *.png
   ```

## Advanced Usage

### Custom Confidence Threshold

To change the confidence threshold, edit `Program.cs`:

```csharp
// In DetectAndRecordSymbols method
if (maxVal >= 0.7)  // Change 0.7 to your preferred threshold (0.0-1.0)
{
    detectedSymbols.Add((symbolName, maxVal));
}
```

### Multiple Symbol Detection

The current implementation detects the best match for each template. For detecting multiple instances of the same symbol, you would need to:
1. Implement non-maximum suppression
2. Use a sliding window approach
3. Record all matches above threshold

This is outside the scope of the current implementation but could be added as an enhancement.

## Summary

The symbol detection feature automatically:
- ✅ Scans every captured image
- ✅ Compares against all symbol templates
- ✅ Records matches with high confidence
- ✅ Updates a CSV file with results
- ✅ Works with both circular screenshots and auto-exported drawings

This enables tracking and logging what symbols/shapes are drawn or captured during camera tracking sessions.
