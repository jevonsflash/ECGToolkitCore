using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ecg.ECGConversion.Extensions
{

    public static class FillRectangleExtensions
    {
        public static IImageProcessingContext FillRectangle(this IImageProcessingContext source, Color color, int x, int y, int width, int height)
        {
            return source.Fill(new SolidBrush(color), new Rectangle(x, y, width, height));
        }


        public static IImageProcessingContext FillRectangle(this IImageProcessingContext source, IBrush brush, int x, int y, int width, int height)
        {
            return source.Fill(brush, new Rectangle(x, y, width, height));
        }


        public static IImageProcessingContext DrawString(this IImageProcessingContext source, string s, Font font, IBrush brush, float x, float y)
        {
            return source.DrawText(s, font, brush, new PointF(x, y));
        }


        public static IImageProcessingContext DrawLine(this IImageProcessingContext source, Pen pen, float x1, float y1, float x2, float y2)
        {

            return source.DrawLines(pen, new PointF(x1, y1), new PointF(x2, y2));
        }

    }
}
