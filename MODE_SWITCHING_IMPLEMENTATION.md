# Mode Switching Implementation

## Overview
This document describes the implementation of dual tracking modes (Drawing Mode and Cursor Mode) in the bright light detection application.

## Problem Statement
The application needed to support two distinct operating modes:
1. **Drawing Mode**: Full features with line drawing, shape detection, and PNG+CSV export
2. **Cursor Mode**: Lightweight coordinate-only tracking with CSV-only export

Additionally, the export behavior should be mode-aware:
- When switching from Drawing → Cursor: Export PNG + CSV
- When switching from Cursor → Drawing: Export CSV only

## Implementation Details

### 1. Added TrackingMode Enum
```csharp
enum TrackingMode
{
    Drawing,  // Drawing mode - draws lines and shapes
    Cursor    // Cursor mode - only tracks coordinates
}
```

### 2. State Management
- Added `currentMode` variable initialized to `TrackingMode.Drawing` (default mode)
- Mode persists across the session and can be toggled with the 'm' key

### 3. Mode-Specific Behavior

#### Drawing Mode Features:
- Orange lines connecting consecutive tracked points
- Real-time shape detection with contour overlay
- Shape name and confidence displayed on screen
- Full export with PNG + CSV when switching to Cursor mode

#### Cursor Mode Features:
- Only cyan dots for tracked points (no connecting lines)
- No shape detection or contour overlay
- CSV-only export when switching to Drawing mode
- Lighter computational load

### 4. Mode Toggle Implementation (Key: 'm')
When 'm' is pressed:
1. Mode switches between Drawing ↔ Cursor
2. Console logs the mode change
3. If points are tracked:
   - **Drawing → Cursor**: Exports PNG + CSV, runs symbol detection, clears points
   - **Cursor → Drawing**: Exports CSV only, clears points
4. File naming convention:
   - Drawing mode files: `drawing_mode_YYYYMMDD_HHMMSS.png/csv`
   - Cursor mode files: `cursor_mode_YYYYMMDD_HHMMSS.csv`

### 5. UI Updates
- Status bar displays current mode: `Mode: DRAWING` or `Mode: CURSOR`
- Console instructions include mode toggle key
- Visual feedback changes based on mode

### 6. Code Quality Improvements

#### Helper Method for Outlier Detection
Extracted common outlier detection logic into a reusable helper:
```csharp
static List<OpenCvSharp.Point> ApplyOutlierDetectionIfEnabled(
    List<OpenCvSharp.Point> points, 
    bool outlierDetectionEnabled)
```
This eliminates code duplication and improves maintainability.

#### Strict Mode Dependency
- Line drawing is now strictly controlled by mode (not mixed with other flags)
- Shape detection only runs in Drawing mode
- Clear separation of concerns

## User Guide

### How to Use Mode Switching

1. **Start the Application**
   ```bash
   cd ColorDetectionApp
   dotnet run
   ```

2. **Default Mode: Drawing**
   - The application starts in Drawing mode
   - Track bright lights with your light source
   - See lines connecting points as you draw
   - See shape detection in real-time

3. **Switch to Cursor Mode**
   - Press 'm' while tracking
   - Current drawing exports as PNG + CSV
   - Mode switches to Cursor
   - Only coordinate tracking (no lines/shapes)

4. **Switch Back to Drawing Mode**
   - Press 'm' while in Cursor mode
   - Current coordinates export as CSV only
   - Mode switches to Drawing
   - Line drawing and shape detection resume

5. **Check Current Mode**
   - Look at the top-left status bar
   - Shows: `Mode: DRAWING` or `Mode: CURSOR`

## File Outputs

### From Drawing Mode (when switching to Cursor):
- `drawing_mode_YYYYMMDD_HHMMSS.png` - Visual representation of the drawing
- `drawing_mode_YYYYMMDD_HHMMSS.csv` - Coordinate data
- Symbol detection results (if shapes detected)

### From Cursor Mode (when switching to Drawing):
- `cursor_mode_YYYYMMDD_HHMMSS.csv` - Coordinate data only

## Testing

### Build Verification
```bash
cd ColorDetectionApp
dotnet build
```
✅ Build succeeds with no errors

### Security Scan
```bash
# Run CodeQL security analysis
```
✅ No security vulnerabilities found

### Code Review
✅ No code quality issues
✅ No code duplication
✅ Clean separation of concerns

## Key Features Retained

The following features work as before:
- Color-based light detection
- Adjustable tracking radius
- Outlier detection (toggle with 'x')
- Circular screenshot capture (key 's')
- Camera flip/mirror (key 'f')
- Fullscreen mode (F11)
- All other existing keyboard shortcuts

## Changes to Existing Behavior

1. **Line Drawing**: Now strictly controlled by mode (previously had mixed behavior with 'a' key)
2. **Shape Detection**: Only active in Drawing mode (previously always active)
3. **Export Behavior**: Now mode-aware with different output based on mode transition

## Benefits

1. **Flexibility**: Users can choose between full-featured drawing and lightweight tracking
2. **Performance**: Cursor mode is lighter without shape detection overhead
3. **Data Organization**: Clear file naming shows which mode produced the data
4. **Clean Separation**: Each mode has distinct, predictable behavior
5. **Maintainability**: Code is well-organized with helper methods and clear logic

## Future Enhancements

Potential improvements for future versions:
- Persist mode preference across sessions
- Add visual indicator (different colors) for mode in the camera view
- Allow mode-specific configuration (e.g., different tracking radii per mode)
- Add statistics tracking per mode
