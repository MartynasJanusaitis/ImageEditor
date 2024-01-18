using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoLooper
{
    internal static class ImageProcessor
    {
        public static Image BoxBlur(Image inputImage, int radius)
        {
            Image outputImage = new Image(inputImage.Width, inputImage.Height);
            int squareSize = (int)Math.Pow(radius * 2 + 1, 2);
            //Blurring only vertical lines
            int[] dynamicSum = null;
            int windowSize = radius * 2 + 1;

            for (int x = 0; x < inputImage.Width; x++)
            {
                Pixel[] col = inputImage.GetCol(x);
                dynamicSum = RangeSum(col, 0, radius + 1);
                outputImage.PutPixel(x, 0, PixelAverage(dynamicSum, windowSize));
                for (int y = 1; y < inputImage.Height; y++)
                {
                    if(y - radius - 1 >= 0)
                    {
                        Pixel prevPixel = col[y - radius - 1];
                        Pixel.Sub(dynamicSum, prevPixel);
                    }

                    if(y + radius < col.Length)
                    {
                        Pixel nextPixel = col[y + radius];
                        Pixel.Add(dynamicSum, nextPixel);
                    }
                    
                    outputImage.PutPixel(x, y, PixelAverage(dynamicSum, windowSize));
                }
            }

            //Blurring only horizontal lines
            for (int y = 0; y < inputImage.Height; y++)
            {
                Pixel[] row = outputImage.GetRow(y);
                dynamicSum = RangeSum(row, 0, radius + 1);
                outputImage.PutPixel(0, y, PixelAverage(dynamicSum, windowSize));
                for (int x = 1; x < inputImage.Width; x++)
                {
                    if (x - radius - 1 >= 0)
                    {
                        Pixel prevPixel = row[x - radius - 1];
                        Pixel.Sub(dynamicSum, prevPixel);
                    }

                    if (x + radius < row.Length)
                    {
                        Pixel nextPixel = row[x + radius];
                        Pixel.Add(dynamicSum, nextPixel);
                    }

                    outputImage.PutPixel(x, y, PixelAverage(dynamicSum, windowSize));
                }
            }

            return outputImage;
        }
        private static int[] RangeSum(Pixel[] pixels, int start, int end)
        {
            int[] output = new int[3];
            Pixel pixel;
            for(int i = start; i < end; i++)
            {
                pixel = pixels[i];
                Pixel.Add(output, pixel);
            }

            return output;
        }
        private static Pixel PixelAverage(int[] sum, int count)
        {
            return new Pixel(
                sum[0] / count,
                sum[1] / count,
                sum[2] / count); 
        }
    }
}
