using System;
using System.Collections.Generic;
using OpenCvSharp;

namespace ColorDetectionApp
{
    public class TestOutlierDebug
    {
        public static void RunDebug()
        {
            // Create points with regular spacing and one huge jump (outlier)
            var points = new List<Point>
            {
                new Point(0, 0),
                new Point(10, 10),    // distance ~14
                new Point(20, 20),    // distance ~14
                new Point(30, 30),    // distance ~14
                new Point(500, 500),  // distance ~664 (OUTLIER!)
                new Point(40, 40),    // distance ~651 (jump back)
                new Point(50, 50),    // distance ~14
                new Point(60, 60)     // distance ~14
            };

            Console.WriteLine("Original points and distances:");
            for (int i = 0; i < points.Count; i++)
            {
                Console.Write($"  Point {i}: ({points[i].X}, {points[i].Y})");
                if (i > 0)
                {
                    int dx = points[i].X - points[i-1].X;
                    int dy = points[i].Y - points[i-1].Y;
                    double dist = Math.Sqrt(dx*dx + dy*dy);
                    Console.Write($" - Distance from previous: {dist:F2}");
                }
                Console.WriteLine();
            }
            
            var stats = OutlierDetection.GetStatistics(points);
            Console.WriteLine($"\n{stats}");
        }
    }
}
