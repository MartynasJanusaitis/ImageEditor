using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoLooper
{
    internal static class ImageProcessor
    {
        // TODO : Fix contrast to work for negative values
        public static Image Contrast(Image inputImage, double value)
        {
            double k = 127.5 * (1 - value);
            Image outputImage = new Image(inputImage.Width, inputImage.Height);
            for (int x = 0; x < inputImage.Width; x++)
            {
                for (int y = 0; y < inputImage.Height; y++)
                {
                    Pixel pixel = inputImage.GetPixel(x, y);
                    outputImage.PutPixel(x, y, new Pixel(
                        Pixel.ClampValue((int)(pixel.r * ((value * pixel.r + k) / 127.5))),
                        Pixel.ClampValue((int)(pixel.g * ((value * pixel.g + k) / 127.5))),
                        Pixel.ClampValue((int)(pixel.b * ((value * pixel.b + k) / 127.5)))));
                }
            }
            return outputImage;
        }
        public static Image Monochrome(Image inputImage)
        {
            Image outputImage = new Image(inputImage.Width, inputImage.Height);

            for (int x = 0; x < inputImage.Width; x++)
            {
                for (int y = 0; y < inputImage.Height; y++)
                {
                    int sum = inputImage.GetPixel(x, y).ValueSum();
                    outputImage.PutPixel(x, y,
                        new Pixel(sum / 3, sum / 3, sum / 3));
                }
            }

            return outputImage;
        }
        // TODO : Implement image saturation
        public static Image Saturation(Image inputImage, double value)
        {
            throw new NotImplementedException();
        }
        // TODO : Implement image sharpening
        public static Image Sharpness(Image inputImage, double value)
        {
            throw new NotImplementedException();
        }
        public static Image Brightness(Image inputImage, double value)
        {
            Image outputImage = new Image(inputImage.Width, inputImage.Height);
            for (int x = 0; x < inputImage.Width; x++)
            {
                for (int y = 0; y < inputImage.Height; y++)
                {
                    outputImage.PutPixel(x, y,
                        inputImage.GetPixel(x, y) * value);
                }
            }
            return outputImage;
        }
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
                    if (y - radius - 1 >= 0)
                    {
                        Pixel prevPixel = col[y - radius - 1];
                        Pixel.Sub(dynamicSum, prevPixel);
                    }

                    if (y + radius < col.Length)
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
        public static Image GenerateGrayscaleHistogram(Image inputImage, int width = 766, int height = 480, int histogramStep = 1)
        {
            return GenerateHistogram(inputImage, width, height, histogramStep, (pixel) => pixel.ValueSum() / 3, new Pixel(255, 255, 255));
        }
        public static Image GenerateRGBHistogram(Image inputImage, int width = 766, int height = 480, int histogramStep = 1)
        {
            return Image.CombineChannels(
                GenerateHistogram(inputImage, width, height, histogramStep, (pixel) => pixel.r, new Pixel(255, 0, 0)),
                GenerateHistogram(inputImage, width, height, histogramStep, (pixel) => pixel.g, new Pixel(0, 255, 0)),
                GenerateHistogram(inputImage, width, height, histogramStep, (pixel) => pixel.b, new Pixel(0, 0, 255)));
        }
        private static Image GenerateHistogram(Image inputImage, int width, int height, int histogramStep, 
                                               Func<Pixel, int> channelSelector, Pixel colorPixel)
        {
            Image histogramImage = new Image(width, height);
            int histogramSize = 255 / histogramStep;
            int[] histogramValues = new int[histogramSize + 1];

            for (int x = 0; x < inputImage.Width; x++)
            {
                for (int y = 0; y < inputImage.Height; y++)
                {
                    int channelValue = channelSelector(inputImage.GetPixel(x, y));
                    histogramValues[channelValue / histogramStep]++;
                }
            }

            int maxValue = histogramValues.Max();

            for (int x = 0; x < width; x++)
            {
                int lineIndex = x * histogramSize / width + 1;
                int lineHeight = height * histogramValues[lineIndex] / maxValue;
                Pixel pixel = new Pixel(0, 0, 0);

                for (int y = 0; y < height; y++)
                {
                    if (y > height - lineHeight)
                    {
                        pixel += colorPixel;
                    }
                    histogramImage.PutPixel(x, y, pixel);
                }
            }

            return histogramImage;
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
