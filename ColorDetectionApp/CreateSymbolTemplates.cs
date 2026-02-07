using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using System;
using System.IO;

namespace ColorDetectionApp
{
    /// <summary>
    /// Utility class to create example symbol templates for testing symbol detection
    /// </summary>
    public class SymbolTemplateGenerator
    {
        public static void GenerateExampleTemplates()
        {
            string symbolsDir = "symbols";
            if (!Directory.Exists(symbolsDir))
            {
                Directory.CreateDirectory(symbolsDir);
            }

            // Create simple geometric shape templates
            CreateCircleTemplate(System.IO.Path.Combine(symbolsDir, "circle.png"));
            CreateSquareTemplate(System.IO.Path.Combine(symbolsDir, "square.png"));
            CreateTriangleTemplate(System.IO.Path.Combine(symbolsDir, "triangle.png"));
            CreateStarTemplate(System.IO.Path.Combine(symbolsDir, "star.png"));
            CreateCrossTemplate(System.IO.Path.Combine(symbolsDir, "cross.png"));

            Console.WriteLine("Example symbol templates created in symbols/ directory");
        }

        private static void CreateCircleTemplate(string path)
        {
            using (var image = new Image<Rgba32>(100, 100))
            {
                image.Mutate(ctx =>
                {
                    ctx.BackgroundColor(Color.Black);
                    var circle = new SixLabors.ImageSharp.Drawing.EllipsePolygon(new PointF(50, 50), 40);
                    ctx.Fill(Color.White, circle);
                });
                image.Save(path);
            }
        }

        private static void CreateSquareTemplate(string path)
        {
            using (var image = new Image<Rgba32>(100, 100))
            {
                image.Mutate(ctx =>
                {
                    ctx.BackgroundColor(Color.Black);
                    var square = new RectangleF(15, 15, 70, 70);
                    ctx.Fill(Color.White, square);
                });
                image.Save(path);
            }
        }

        private static void CreateTriangleTemplate(string path)
        {
            using (var image = new Image<Rgba32>(100, 100))
            {
                image.Mutate(ctx =>
                {
                    ctx.BackgroundColor(Color.Black);
                    var triangle = new Polygon(new LinearLineSegment(
                        new PointF(50, 10),
                        new PointF(90, 90),
                        new PointF(10, 90)
                    ));
                    ctx.Fill(Color.White, triangle);
                });
                image.Save(path);
            }
        }

        private static void CreateStarTemplate(string path)
        {
            using (var image = new Image<Rgba32>(100, 100))
            {
                image.Mutate(ctx =>
                {
                    ctx.BackgroundColor(Color.Black);
                    // Create a 5-pointed star
                    var points = new PointF[]
                    {
                        new PointF(50, 10),  // top
                        new PointF(61, 40),  // inner right top
                        new PointF(90, 40),  // outer right
                        new PointF(68, 60),  // inner right bottom
                        new PointF(79, 90),  // outer bottom right
                        new PointF(50, 73),  // inner bottom
                        new PointF(21, 90),  // outer bottom left
                        new PointF(32, 60),  // inner left bottom
                        new PointF(10, 40),  // outer left
                        new PointF(39, 40)   // inner left top
                    };
                    var star = new Polygon(new LinearLineSegment(points));
                    ctx.Fill(Color.White, star);
                });
                image.Save(path);
            }
        }

        private static void CreateCrossTemplate(string path)
        {
            using (var image = new Image<Rgba32>(100, 100))
            {
                image.Mutate(ctx =>
                {
                    ctx.BackgroundColor(Color.Black);
                    // Vertical bar
                    ctx.Fill(Color.White, new RectangleF(40, 10, 20, 80));
                    // Horizontal bar
                    ctx.Fill(Color.White, new RectangleF(10, 40, 80, 20));
                });
                image.Save(path);
            }
        }
    }
}
