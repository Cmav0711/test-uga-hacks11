using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
                Console.WriteLine("Usage: ColorDetectionApp <image_path>");
                Console.WriteLine("\nGenerating a sample image for demonstration...");
                GenerateSampleImage("sample_input.png");
                args = new string[] { "sample_input.png" };
            }

            string imagePath = args[0];

            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"Error: Image file '{imagePath}' not found.");
                return;
            }

            ProcessImage(imagePath);
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
                foreach (var region in brightRegions)
                {
                    detectionCount++;

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
