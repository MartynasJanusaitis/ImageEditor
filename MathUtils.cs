using ImageEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    internal static class MathUtils
    {
        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }
        /// <summary>
        /// Returns the sum of rgb channels from a range of pixels
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>Array of size 3 containing the respective rgb channel sums</returns>
        public static int[] RangeSum(Pixel[] pixels, int start, int end)
        {
            int[] output = new int[3];
            Pixel pixel;
            for (int i = start; i < end; i++)
            {
                pixel = pixels[i];
                Pixel.Add(output, pixel);
            }

            return output;
        }
    }
}
