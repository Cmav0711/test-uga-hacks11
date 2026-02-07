using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace ColorDetectionApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bright Light Color Detection and Mapping");
            Console.WriteLine("========================================\n");

            if (args.Length == 0)
            {
                Console.WriteLine("Starting real-time camera tracking...");
                Console.WriteLine("Press 'q' to quit, 'c' to clear tracked points\n");
                RunCameraTracking();
                return;
            }

            string imagePath = args[0];

            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"Error: Image file '{imagePath}' not found.");
                return;
            }

            ProcessImage(imagePath);
        }

        static void RunCameraTracking()
        {
            // List to store all brightest points from previous frames
            var brightestPoints = new List<OpenCvSharp.Point>();
            
            // Calibration data - stores baseline color for comparison
            Scalar? calibratedColor = null;
            
            // Tracking radius for filtering points (adjustable via +/- keys)
            int trackingRadius = 100;
            
            // Open the default camera
            using (var capture = new VideoCapture(0))
            {
                if (!capture.IsOpened())
                {
                    Console.WriteLine("Error: Could not open camera. Make sure a camera is connected.");
                    return;
                }

                // Set camera properties for better performance
                capture.Set(VideoCaptureProperties.FrameWidth, 640);
                capture.Set(VideoCaptureProperties.FrameHeight, 480);

                Console.WriteLine($"Camera opened successfully!");
                Console.WriteLine($"Resolution: {capture.FrameWidth}x{capture.FrameHeight}");
                Console.WriteLine("Press 'b' to calibrate with brightest point color");
                Console.WriteLine("Press '+' to increase tracking radius, '-' to decrease");

                using (var frame = new Mat())
                using (var window = new Window("Brightest Point Tracker - Press 'q' to quit, 'c' to clear, 'b' to calibrate, '+/-' for radius"))
                {
                    while (true)
                    {
                        // Capture frame from camera
                        capture.Read(frame);
                        
                        if (frame.Empty())
                        {
                            Console.WriteLine("Warning: Empty frame captured");
                            break;
                        }

                        // Find the brightest point in this frame
                        // If we have previous points, only search within the tracking circle
                        var searchCenter = brightestPoints.Count > 0 
                            ? brightestPoints[brightestPoints.Count - 1] 
                            : (OpenCvSharp.Point?)null;
                        var brightestPoint = FindBrightestPoint(frame, searchCenter, trackingRadius);
                        
                        if (brightestPoint.HasValue)
                        {
                            // Add to our collection (already filtered by circle search)
                            brightestPoints.Add(brightestPoint.Value);
                            
                            // Get the color at the brightest point
                            var color = GetColorAtPoint(frame, brightestPoint.Value);
                            
                            // Draw the current brightest point (large, bright green)
                            var pointColor = new Scalar(0, 255, 0);
                            Cv2.Circle(frame, brightestPoint.Value, 8, pointColor, -1);
                            Cv2.Circle(frame, brightestPoint.Value, 10, pointColor, 2);
                            
                            // Display color information
                            string colorInfo = $"Color: B={color.Val0:F0} G={color.Val1:F0} R={color.Val2:F0}";
                            Cv2.PutText(frame, colorInfo, new OpenCvSharp.Point(10, 60), 
                                       HersheyFonts.HersheySimplex, 0.5, new Scalar(255, 255, 255), 1);
                            
                            // If calibrated, show color difference
                            if (calibratedColor.HasValue)
                            {
                                double colorDiff = CalculateColorDifference(color, calibratedColor.Value);
                                string diffInfo = $"Color Diff: {colorDiff:F1}";
                                Cv2.PutText(frame, diffInfo, new OpenCvSharp.Point(10, 85), 
                                           HersheyFonts.HersheySimplex, 0.5, new Scalar(255, 255, 255), 1);
                            }
                        }

                        // Draw lines connecting consecutive points
                        for (int i = 1; i < brightestPoints.Count; i++)
                        {
                            Cv2.Line(frame, brightestPoints[i - 1], brightestPoints[i], 
                                    new Scalar(255, 128, 0), 2);
                        }

                        // Draw all historical points (smaller, cyan)
                        foreach (var point in brightestPoints)
                        {
                            Cv2.Circle(frame, point, 3, new Scalar(255, 255, 0), -1);
                        }
                        
                        // Draw tracking radius circle around the last tracked point
                        if (brightestPoints.Count > 0)
                        {
                            var lastPoint = brightestPoints[brightestPoints.Count - 1];
                            Cv2.Circle(frame, lastPoint, trackingRadius, new Scalar(255, 0, 255), 2);
                        }

                        // Display frame count and point count
                        string info = $"Points tracked: {brightestPoints.Count} | Tracking radius: {trackingRadius}px";
                        if (calibratedColor.HasValue)
                        {
                            info += " [CALIBRATED]";
                        }
                        Cv2.PutText(frame, info, new OpenCvSharp.Point(10, 30), 
                                   HersheyFonts.HersheySimplex, 0.7, new Scalar(255, 255, 255), 2);

                        // Show the frame
                        window.ShowImage(frame);

                        // Check for key press
                        int key = Cv2.WaitKey(1);
                        if (key == 'q' || key == 'Q' || key == 27) // 'q' or ESC
                        {
                            break;
                        }
                        else if (key == 'c' || key == 'C')
                        {
                            brightestPoints.Clear();
                            Console.WriteLine("Cleared all tracked points");
                        }
                        else if (key == 'b' || key == 'B')
                        {
                            // Calibration: capture the color of the brightest point
                            if (brightestPoint.HasValue)
                            {
                                calibratedColor = GetColorAtPoint(frame, brightestPoint.Value);
                                Console.WriteLine($"Calibrated! Baseline color: B={calibratedColor.Value.Val0:F0} G={calibratedColor.Value.Val1:F0} R={calibratedColor.Value.Val2:F0}");
                            }
                            else
                            {
                                Console.WriteLine("No bright point found for calibration");
                            }
                        }
                        else if (key == '+' || key == '=')
                        {
                            trackingRadius += 10;
                            if (trackingRadius > 500) trackingRadius = 500;
                            Console.WriteLine($"Tracking radius increased to {trackingRadius}px");
                        }
                        else if (key == '-' || key == '_')
                        {
                            trackingRadius -= 10;
                            if (trackingRadius < 10) trackingRadius = 10;
                            Console.WriteLine($"Tracking radius decreased to {trackingRadius}px");
                        }
                    }
                }

                Console.WriteLine($"\nTotal points tracked: {brightestPoints.Count}");
                Console.WriteLine("Camera tracking stopped.");
            }
        }

        static OpenCvSharp.Point? FindBrightestPoint(Mat frame, OpenCvSharp.Point? circleCenter = null, int circleRadius = 0)
        {
            // Convert to grayscale for easier brightness analysis
            using (var gray = new Mat())
            {
                Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
                
                Mat? mask = null;
                try
                {
                    // If we have a circle center, create a mask for the circular region
                    if (circleCenter.HasValue && circleRadius > 0)
                    {
                        mask = new Mat(gray.Size(), MatType.CV_8UC1, new Scalar(0));
                        // Draw a filled white circle on the mask
                        Cv2.Circle(mask, circleCenter.Value, circleRadius, new Scalar(255), -1);
                    }
                    
                    // Find the location of the maximum brightness (within the mask if provided)
                    double minVal, maxVal;
                    OpenCvSharp.Point minLoc, maxLoc;
                    Cv2.MinMaxLoc(gray, out minVal, out maxVal, out minLoc, out maxLoc, mask);
                    
                    // Only return if brightness is above threshold (to avoid noise in dark scenes)
                    if (maxVal >= 200)
                    {
                        return maxLoc;
                    }
                }
                finally
                {
                    mask?.Dispose();
                }
            }
            
            return null;
        }

        static Scalar GetColorAtPoint(Mat frame, OpenCvSharp.Point point)
        {
            // Ensure the point is within the frame bounds
            if (point.X >= 0 && point.X < frame.Width && point.Y >= 0 && point.Y < frame.Height)
            {
                // Get the color value at the specified point (BGR format in OpenCV)
                // Note: OpenCV uses row-major indexing (row, column) so Y comes before X
                Vec3b color = frame.At<Vec3b>(point.Y, point.X);
                return new Scalar(color.Item0, color.Item1, color.Item2);
            }
            return new Scalar(0, 0, 0);
        }

        static double CalculateColorDifference(Scalar color1, Scalar color2)
        {
            // Calculate Euclidean distance in BGR color space
            // Note: BGR color space is not perceptually uniform. For better results matching
            // human perception, consider using CIE Lab or CIE Delta E color spaces.
            double bDiff = color1.Val0 - color2.Val0;
            double gDiff = color1.Val1 - color2.Val1;
            double rDiff = color1.Val2 - color2.Val2;
            return Math.Sqrt(bDiff * bDiff + gDiff * gDiff + rDiff * rDiff);
        }

        static double CalculateDistance(OpenCvSharp.Point p1, OpenCvSharp.Point p2)
        {
            // Calculate Euclidean distance between two points
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        static void GenerateSampleImage(string outputPath)
        {
            // Create a sample image with bright spots
            int width = 640;
            int height = 480;
            
            using (var image = new Image<Rgba32>(width, height))
            {
                // Fill with dark background
                image.Mutate(ctx => ctx.BackgroundColor(new Rgba32(30, 30, 30)));

                // Add some bright light spots (white/yellow)
                image.Mutate(ctx =>
                {
                    ctx.Fill(new Rgba32(255, 255, 0), new RectangleF(135, 135, 30, 30)); // Yellow circle approx
                    ctx.Fill(new Rgba32(255, 255, 255), new RectangleF(387, 187, 26, 26)); // White
                    ctx.Fill(new Rgba32(255, 255, 200), new RectangleF(290, 340, 20, 20)); // Light yellow
                    ctx.Fill(new Rgba32(200, 255, 255), new RectangleF(482, 82, 36, 36)); // Light cyan
                    ctx.Fill(new Rgba32(255, 255, 255), new RectangleF(85, 385, 30, 30)); // White
                });

                image.Save(outputPath);
                Console.WriteLine($"Sample image generated: {outputPath}\n");
            }
        }

        static void ProcessImage(string imagePath)
        {
            Console.WriteLine($"Processing image: {imagePath}");

            using (var image = Image.Load<Rgba32>(imagePath))
            {
                Console.WriteLine($"Image size: {image.Width}x{image.Height}");

                // Detect bright regions
                var brightRegions = DetectBrightRegions(image);

                Console.WriteLine($"Found {brightRegions.Count} bright light region(s)");

                // Create output images
                var outputMap = image.Clone();
                var pointMap = new Image<Rgba32>(image.Width, image.Height);
                pointMap.Mutate(ctx => ctx.BackgroundColor(new Rgba32(0, 0, 0)));

                int detectionCount = 0;
                var centerPoints = new List<(int x, int y)>();
                
                foreach (var region in brightRegions)
                {
                    detectionCount++;
                    centerPoints.Add((region.CenterX, region.CenterY));

                    Console.WriteLine($"  Light #{detectionCount}:");
                    Console.WriteLine($"    Position: ({region.CenterX}, {region.CenterY})");
                    Console.WriteLine($"    Area: {region.PixelCount} pixels");
                    Console.WriteLine($"    Bounding Box: ({region.MinX}, {region.MinY}) - {region.MaxX - region.MinX}x{region.MaxY - region.MinY}");

                    // Draw on output map
                    outputMap.Mutate(ctx =>
                    {
                        // Draw bounding box
                        var rect = new RectangleF(region.MinX, region.MinY, region.MaxX - region.MinX, region.MaxY - region.MinY);
                        ctx.Draw(new Rgba32(255, 0, 0), 2, rect);

                        // Draw center point with a small circle
                        var centerCircle = new RectangleF(region.CenterX - 5, region.CenterY - 5, 10, 10);
                        ctx.Fill(new Rgba32(0, 0, 255), centerCircle);
                    });

                    // Draw on point map
                    pointMap.Mutate(ctx =>
                    {
                        var pointCircle = new RectangleF(region.CenterX - 10, region.CenterY - 10, 20, 20);
                        ctx.Fill(new Rgba32(0, 255, 255), pointCircle);
                    });
                }
                
                // Draw lines connecting consecutive points
                if (centerPoints.Count > 1)
                {
                    outputMap.Mutate(ctx =>
                    {
                        for (int i = 1; i < centerPoints.Count; i++)
                        {
                            var p1 = new PointF(centerPoints[i - 1].x, centerPoints[i - 1].y);
                            var p2 = new PointF(centerPoints[i].x, centerPoints[i].y);
                            ctx.DrawLine(new Rgba32(0, 255, 0), 2, p1, p2);
                        }
                    });
                    
                    pointMap.Mutate(ctx =>
                    {
                        for (int i = 1; i < centerPoints.Count; i++)
                        {
                            var p1 = new PointF(centerPoints[i - 1].x, centerPoints[i - 1].y);
                            var p2 = new PointF(centerPoints[i].x, centerPoints[i].y);
                            ctx.DrawLine(new Rgba32(255, 255, 0), 2, p1, p2);
                        }
                    });
                }

                // Save outputs
                string baseName = Path.GetFileNameWithoutExtension(imagePath);
                string? outputDir = Path.GetDirectoryName(imagePath);
                if (string.IsNullOrEmpty(outputDir))
                    outputDir = ".";

                string outputPath = Path.Combine(outputDir, $"{baseName}_detected.png");
                string pointMapPath = Path.Combine(outputDir, $"{baseName}_point_map.png");

                outputMap.Save(outputPath);
                pointMap.Save(pointMapPath);

                Console.WriteLine($"\nOutput files created:");
                Console.WriteLine($"  Detection overlay: {outputPath}");
                Console.WriteLine($"  Point map: {pointMapPath}");

                outputMap.Dispose();
                pointMap.Dispose();
            }

            Console.WriteLine("\nProcessing complete!");
        }

        static List<BrightRegion> DetectBrightRegions(Image<Rgba32> image)
        {
            var regions = new List<BrightRegion>();
            var visited = new bool[image.Width, image.Height];
            int brightnessThreshold = 200; // Threshold for considering a pixel as "bright"

            // Scan the image for bright pixels
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (visited[x, y])
                        continue;

                    var pixel = image[x, y];
                    
                    // Check if pixel is bright (high RGB values, low saturation difference)
                    int maxChannel = Math.Max(Math.Max(pixel.R, pixel.G), pixel.B);
                    int minChannel = Math.Min(Math.Min(pixel.R, pixel.G), pixel.B);
                    int saturation = maxChannel - minChannel;

                    if (maxChannel >= brightnessThreshold && saturation < 100)
                    {
                        // Found a bright pixel, flood fill to find the entire region
                        var region = FloodFill(image, visited, x, y, brightnessThreshold);
                        
                        // Filter out small regions (noise)
                        if (region.PixelCount >= 50)
                        {
                            regions.Add(region);
                        }
                    }
                }
            }

            return regions;
        }

        static BrightRegion FloodFill(Image<Rgba32> image, bool[,] visited, int startX, int startY, int brightnessThreshold)
        {
            var region = new BrightRegion();
            var queue = new Queue<(int x, int y)>();
            queue.Enqueue((startX, startY));
            visited[startX, startY] = true;

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                region.AddPixel(x, y);

                // Check 4-connected neighbors
                foreach (var (dx, dy) in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < image.Width && ny >= 0 && ny < image.Height && !visited[nx, ny])
                    {
                        var pixel = image[nx, ny];
                        int maxChannel = Math.Max(Math.Max(pixel.R, pixel.G), pixel.B);
                        int minChannel = Math.Min(Math.Min(pixel.R, pixel.G), pixel.B);
                        int saturation = maxChannel - minChannel;

                        if (maxChannel >= brightnessThreshold && saturation < 100)
                        {
                            visited[nx, ny] = true;
                            queue.Enqueue((nx, ny));
                        }
                    }
                }
            }

            region.CalculateCenter();
            return region;
        }
    }

    class BrightRegion
    {
        public int MinX { get; private set; } = int.MaxValue;
        public int MaxX { get; private set; } = int.MinValue;
        public int MinY { get; private set; } = int.MaxValue;
        public int MaxY { get; private set; } = int.MinValue;
        public int PixelCount { get; private set; }
        public int CenterX { get; private set; }
        public int CenterY { get; private set; }

        private int sumX = 0;
        private int sumY = 0;

        public void AddPixel(int x, int y)
        {
            MinX = Math.Min(MinX, x);
            MaxX = Math.Max(MaxX, x);
            MinY = Math.Min(MinY, y);
            MaxY = Math.Max(MaxY, y);
            sumX += x;
            sumY += y;
            PixelCount++;
        }

        public void CalculateCenter()
        {
            if (PixelCount > 0)
            {
                CenterX = sumX / PixelCount;
                CenterY = sumY / PixelCount;
            }
        }
    }
}
