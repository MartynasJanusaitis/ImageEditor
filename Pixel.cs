using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    public class Pixel : IEquatable<Pixel>
    {
        public byte r { get; private set; }
        public byte g { get; private set; }
        public byte b { get; private set; }
        public byte a { get; private set; }

        public Pixel()
        {
            r = 0;
            g = 0;
            b = 0;
            a = 255;
        }
        public Pixel(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = 255;
        }
        public Pixel(int r, int g, int b)
        {
            this.r = ClampValue(r);
            this.g = ClampValue(g);
            this.b = ClampValue(b);
            a = 255;
        }
        public Pixel(byte r, byte g, byte b, byte a) : this(r, g, b)
        {
            this.a = a;
        }
        public Pixel(int r, int g, int b, int a) : this(r, g, b)
        {
            this.a = ClampValue(a);
        }
        public Pixel(Pixel pixel) : this(pixel.r, pixel.g, pixel.b, pixel.a)
        {

        }
        public bool Equals(Pixel other)
        {
            if (other is null) return false;
            return this.a == other.a &&
                this.r == other.r &&
                this.g == other.g &&
                this.b == other.b;
        }
        public static Pixel operator +(Pixel pixelA, Pixel pixelB)
        {
            return new Pixel(
                ClampValue(pixelA.r + pixelB.r),
                ClampValue(pixelA.g + pixelB.g),
                ClampValue(pixelA.b + pixelB.b));
        }
        public static Pixel operator -(Pixel pixelA, Pixel pixelB)
        {
            return new Pixel(
                ClampValue(pixelA.r - pixelB.r),
                ClampValue(pixelA.g - pixelB.g),
                ClampValue(pixelA.b - pixelB.b));
        }
        public static Pixel operator *(Pixel pixelA, double pixelB)
        {
            return new Pixel(
                ClampValue((int)(pixelA.r * pixelB)),
                ClampValue((int)(pixelA.g * pixelB)),
                ClampValue((int)(pixelA.b * pixelB)),
                pixelA.a);
        }
        public static Pixel operator *(Pixel pixelA, Pixel pixelB)
        {
            return new Pixel(
                ClampValue((int)(pixelA.r * pixelB.r)),
                ClampValue((int)(pixelA.g * pixelB.r)),
                ClampValue((int)(pixelA.b * pixelB.r)),
                pixelA.a);
        }
        public static bool operator ==(Pixel pixelA, Pixel pixelB)
        {
            return pixelA.Equals(pixelB);
        }
        public static bool operator !=(Pixel pixelA, Pixel pixelB)
        {
            return !pixelA.Equals(pixelB);
        }
        public static byte ClampValue(int value, int min = 0, int max = 255)
        {
            if (value < min) value = 0;
            else if (value > max) value = 255;
            return (byte)value;
        }
        public int ValueSum()
        {
            return r + g + b;
        }
        public static void Add(int[] arr, Pixel pixel)
        {
            arr[0] += pixel.r;
            arr[1] += pixel.g;
            arr[2] += pixel.b;
        }
        public static void Sub(int[] arr, Pixel pixel)
        {
            arr[0] -= pixel.r;
            arr[1] -= pixel.g;
            arr[2] -= pixel.b;
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
        public static Pixel ApplyMatrixToPixel(Image inputImage, int xIndex, int yIndex, double[,] matrix, double divisor = 0)
        {
            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);

            double[] sum = new double[3];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Pixel pixel = inputImage.GetPixel(x + xIndex - width / 2,
                                                      y + yIndex - height / 2);
                    if (pixel == null) continue;
                    sum[0] += pixel.r * matrix[x, y];
                    sum[1] += pixel.g * matrix[x, y];
                    sum[2] += pixel.b * matrix[x, y];
                }
            }

            if (divisor == 0) divisor = height * width;

            return new Pixel(
                (int)(sum[0] / divisor),
                (int)(sum[1] / divisor),
                (int)(sum[2] / divisor));
        }
        /// <summary>
        /// Returns an average value pixel based on given sum array
        /// </summary>
        /// <param name="sum">Array containing the sums of the rgb channels</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Pixel PixelAverage(int[] sum, int count)
        {
            return new Pixel(
                sum[0] / count,
                sum[1] / count,
                sum[2] / count);
        }



    }
}
