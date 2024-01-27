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

namespace ImageEditor
{
    internal static class ImageProcessor
    {
        // TODO : Implement contrast
        public static Image Contrast(Image inputImage, double value)
        {
            throw new NotImplementedException();
        }
        public static Image Monochrome(Image inputImage)
        {
            return Image.ApplyFunction(inputImage, (pixel) => 
                new Pixel(pixel.ValueSum() / 3, 
                          pixel.ValueSum() / 3, 
                          pixel.ValueSum() / 3));
        }
        // TODO : Implement image saturation
        public static Image Saturation(Image inputImage, double value)
        {
            throw new NotImplementedException();
        }
        public static Image Sharpness(Image inputImage)
        {
            double[,] sharpnessMatrix = { 
                { 0, -1, 0 }, 
                { -1, 5, -1}, 
                { 0, -1, 0 } };
            return Image.ApplyConvolutionMatrix(inputImage, sharpnessMatrix, 1);
        }
        public static Image EdgeDetection(Image inputImage)
        {
            double[,] sharpnessMatrix = {
                { 0, -1, 0 },
                { -1, 4, -1},
                { 0, -1, 0 } };
            return Image.ApplyConvolutionMatrix(inputImage, sharpnessMatrix, 1);
        }
        public static Image Brightness(Image inputImage, double value)
        {
            return Image.ApplyFunction(inputImage, (pixel) => pixel * (1 + value));
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
                dynamicSum = MathUtils.RangeSum(col, 0, radius + 1);
                outputImage.PutPixel(x, 0, Pixel.PixelAverage(dynamicSum, windowSize));
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

                    outputImage.PutPixel(x, y, Pixel.PixelAverage(dynamicSum, windowSize));
                }
            }

            //Blurring only horizontal lines
            for (int y = 0; y < inputImage.Height; y++)
            {
                Pixel[] row = outputImage.GetRow(y);
                dynamicSum = MathUtils.RangeSum(row, 0, radius + 1);
                outputImage.PutPixel(0, y, Pixel.PixelAverage(dynamicSum, windowSize));
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

                    outputImage.PutPixel(x, y, Pixel.PixelAverage(dynamicSum, windowSize));
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
        /// <summary>
        /// Generates a brightness value histogram for a single color channel
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="width">Histogram output image width</param>
        /// <param name="height">Histogram output image height</param>
        /// <param name="histogramStep">Number of brightness values to combine into one column</param>
        /// <param name="channelSelector">Func which selects which channels to use</param>
        /// <param name="colorPixel">The color of the histogram</param>
        /// <returns>Histogram image</returns>
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
    }
}
