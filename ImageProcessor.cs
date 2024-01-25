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
        // TODO : Implement contrast
        public static Image Contrast(Image inputImage, double value)
        {
            throw new NotImplementedException();
        }
        public static Image Monochrome(Image inputImage)
        {
            return ApplyFunction(inputImage, (pixel) => 
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
            return ApplyConvolutionMatrix(inputImage, sharpnessMatrix, 1);
        }
        public static Image EdgeDetection(Image inputImage)
        {
            double[,] sharpnessMatrix = {
                { 0, -1, 0 },
                { -1, 4, -1},
                { 0, -1, 0 } };
            return ApplyConvolutionMatrix(inputImage, sharpnessMatrix, 1);
        }
        public static Image Brightness(Image inputImage, double value)
        {
            return ApplyFunction(inputImage, (pixel) => pixel * (1 + value));
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
            return CombineChannels(
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
        /// <summary>
        /// Applies function to every pixel in the image
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="func"></param>
        /// <returns>Modified image</returns>
        private static Image ApplyFunction(Image inputImage, Func<Pixel, Pixel> func)
        {
            Image outputImage = new Image(inputImage.Width, inputImage.Height);
            for (int x = 0; x < inputImage.Width; x++)
            {
                for (int y = 0; y < inputImage.Height; y++)
                {
                    outputImage.PutPixel(x, y, 
                        func(inputImage.GetPixel(x, y)));
                }
            }
            return outputImage;
        }
        private static Image ApplyConvolutionMatrix(Image inputImage, double[,] matrix, double divisor)
        {
            Image outputImage = new Image(inputImage.Width, inputImage.Height);

            for(int x = 0; x < inputImage.Width; x++)
            {
                for(int y = 0; y < inputImage.Height; y++)
                {
                    outputImage.PutPixel(x, y,
                        ApplyMatrixToPixel(inputImage, x, y, matrix, divisor));
                }
            }

            return outputImage;
        }
        /// <summary>
        /// Applies matrix to a single pixel
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="xIndex"></param>
        /// <param name="yIndex"></param>
        /// <param name="matrix"></param>
        /// <param name="divisor"></param>
        /// <returns>Modified pixel</returns>
        private static Pixel ApplyMatrixToPixel(Image inputImage, int xIndex, int yIndex, double[,] matrix, double divisor = 0)
        {
            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);

            double[] sum = new double[3];

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    Pixel pixel = inputImage.GetPixel(x + xIndex - width / 2,
                                                      y + yIndex - height / 2);
                    if(pixel == null) continue;
                    sum[0] += pixel.r * matrix[x, y];
                    sum[1] += pixel.g * matrix[x, y];
                    sum[2] += pixel.b * matrix[x, y];
                }
            }

            if(divisor == 0) divisor = height * width;

            return new Pixel(
                (int)(sum[0] / divisor),
                (int)(sum[1] / divisor),
                (int)(sum[2] / divisor));
        }
        /// <summary>
        /// Returns the sum of rgb channels from a range of pixels
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>Array of size 3 containing the respective rgb channel sums</returns>
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
        /// <summary>
        /// Returns an average value pixel based on given sum array
        /// </summary>
        /// <param name="sum">Array containing the sums of the rgb channels</param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static Pixel PixelAverage(int[] sum, int count)
        {
            return new Pixel(
                sum[0] / count,
                sum[1] / count,
                sum[2] / count); 
        }
        /// <summary>
        /// Combines the respective rgb channels of three images
        /// </summary>
        /// <param name="red">Image containing the red channel</param>
        /// <param name="green">Image containing the green channel</param>
        /// <param name="blue">Image containing the blue channel</param>
        /// <returns>Image with combined channels</returns>
        private static Image CombineChannels(Image red, Image green, Image blue)
        {
            Image outputImage = new Image(
            Math.Max(red.Width, Math.Max(green.Width, blue.Width)),
            Math.Max(red.Height, Math.Max(green.Height, blue.Height)));

            for (int x = 0; x < outputImage.Width; x++)
            {
                for (int y = 0; y < outputImage.Height; y++)
                {
                    outputImage.PutPixel(x, y,
                        red.GetPixel(x, y) +
                        green.GetPixel(x, y) +
                        blue.GetPixel(x, y));
                }
            }

            return outputImage;
        }
    }
}
