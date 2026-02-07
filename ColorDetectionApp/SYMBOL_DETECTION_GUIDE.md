# Symbol Detection Usage Guide

This guide demonstrates how to use the folder-based symbol training and detection feature in the Bright Light Color Detection Application.

## Quick Start

### 1. Generate Symbol Training Folders (First Time Only)

Before using symbol detection, generate example symbol folders with training images:

```bash
cd ColorDetectionApp
dotnet run --generate-symbols
```

This creates 5 example symbol folders in the `symbols/` directory, each containing 4 training variations:
- `circle/` - Multiple circle variations (different sizes, positions)
- `square/` - Multiple square variations (including rotated)
- `triangle/` - Multiple triangle variations (including inverted)
- `star/` - Multiple 5-pointed star variations
- `cross/` - Multiple cross/plus sign variations

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
| **DetectedSymbols** | The folder name of the best matching symbol | `circle` or `None` |
| **Confidence** | Average confidence score across all training images | `0.892` or `N/A` |
| **Count** | Always 1 when a symbol is detected | `1` or `0` |

### Example CSV Content

```csv
Timestamp,ImageFilename,DetectedSymbols,Confidence,Count
2024-02-07 18:15:23,circular_screenshot_20240207_181523.png,circle,0.892,1
2024-02-07 18:16:45,light_drawing_20240207_181645.png,square,0.812,1
2024-02-07 18:18:12,circular_screenshot_20240207_181812.png,triangle,0.654,1
```

## Customizing Symbol Training

### Adding Your Own Symbols

1. Create a new folder in the `symbols/` directory with your symbol's name
2. Add multiple training images inside the folder showing variations of your symbol
3. Use high contrast images (white symbol on black background works best)
4. The folder name becomes the symbol name in detection results

Example:
```
symbols/
  ├── heart/
  │   ├── heart_1.png      # Perfect heart
  │   ├── heart_2.png      # Smaller heart
  │   ├── heart_3.png      # Messy/hand-drawn heart
  │   └── heart_4.png      # Rotated heart
  ├── arrow/
  │   ├── arrow_up.png
  │   ├── arrow_right.png
  │   └── arrow_left.png
  └── letter_A/
      ├── A_variation1.png
      ├── A_variation2.png
      └── A_variation3.png
```

### Training Image Best Practices

For best detection results with multiple training images:
- **Multiple Variations**: Include 3-5 images per symbol showing different sizes, positions, or slight distortions
- **High Contrast**: White symbols on black background (or vice versa)
- **Represent Reality**: Include images that look like what users might actually draw (including messy versions)
- **Consistent Style**: Keep all training images in a similar style within each folder
- **Clean Edges**: Sharp, clear boundaries work better than fuzzy edges

## How Symbol Detection Works

The application uses multi-template ensemble matching:

1. **Load Training Images**: Reads all PNG/JPG/JPEG files from each symbol folder in `symbols/` directory
2. **Convert to Grayscale**: Both captured image and all training images are converted to grayscale
3. **Template Matching**: Uses CCoeffNormed matching method for each training image
4. **Average Confidence**: Calculates the average confidence score across all training images in each symbol folder
5. **Best Match Selection**: Selects the symbol folder with the highest average confidence
6. **Record Results**: Appends the best matching folder name to `detected_symbols.csv`

### Benefits of Multi-Template Training

- **Robustness**: Training on multiple variations makes detection more reliable
- **Handles Imperfections**: Better at recognizing messy or hand-drawn symbols
- **Size/Position Invariant**: Multiple training images help recognize symbols at different scales and positions
- **Ensemble Voting**: Averaging across multiple templates reduces false positives

### Confidence Scores

- **0.90 - 1.00**: Excellent match, very high confidence
- **0.80 - 0.89**: Good match, high confidence
- **0.70 - 0.79**: Good match, moderate-high confidence
- **0.50 - 0.69**: Acceptable match, moderate confidence
- **0.30 - 0.49**: Weak match, low confidence
- **< 0.30**: Very weak match

## Testing Symbol Detection

Run the built-in test to verify everything works:

```bash
dotnet run --test-symbols
```

This will:
1. Generate symbol folders with training images (if not already present)
2. Create a test image with a circle
3. Run symbol detection on the test image
4. Report results

## Troubleshooting

### No Symbols Detected

If symbols aren't being detected:
- Ensure `symbols/` directory exists and contains symbol folders
- Verify each symbol folder contains at least one training image
- Check that captured images have clear, visible symbols
- Verify training images match the style/appearance of captured symbols

### Adding More Training Images

To improve detection accuracy for a specific symbol:
1. Navigate to the symbol's folder (e.g., `symbols/circle/`)
2. Add more PNG/JPG images showing different variations
3. The system will automatically use all images for training

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

### Adding Custom Symbols with Your Own Training Images

To add a completely new symbol:

1. Create a new folder in `symbols/` with the symbol name:
   ```bash
   mkdir symbols/my_symbol
   ```

2. Add your training images (3-5 recommended):
   - Take photos or create drawings of the symbol in different styles
   - Include variations: perfect, messy, different sizes
   - Save as PNG or JPG files in the folder
   
3. The system automatically uses your new symbol folder - no code changes needed!

### Example: Adding a Heart Symbol

```bash
mkdir symbols/heart
# Add your training images
cp heart_perfect.png symbols/heart/heart_1.png
cp heart_messy.png symbols/heart/heart_2.png
cp heart_small.png symbols/heart/heart_3.png
cp heart_large.png symbols/heart/heart_4.png
```

Now when you capture an image, the system will also check if it matches a heart!

### Multiple Symbol Detection

The current implementation detects the best match for each template. For detecting multiple instances of the same symbol, you would need to:
1. Implement non-maximum suppression
2. Use a sliding window approach
3. Record all matches above threshold

This is outside the scope of the current implementation but could be added as an enhancement.

## Summary

The folder-based symbol training and detection feature:
- ✅ Organizes training images in folders by symbol name
- ✅ Trains on multiple image variations per symbol
- ✅ Calculates average confidence across all training images
- ✅ Returns the folder name (symbol) with highest confidence
- ✅ More robust to messy, imperfect, or hand-drawn symbols
- ✅ Works with both circular screenshots and auto-exported drawings
- ✅ Easy to add new symbols - just create a folder and add training images

This enables accurate tracking and logging of what symbols/shapes are drawn or captured during camera tracking sessions, even when drawings are messy or imperfect.
