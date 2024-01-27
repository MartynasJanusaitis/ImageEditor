using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    internal class Pixel
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
        public static Pixel operator +(Pixel x, Pixel y)
        {
            return new Pixel(
                ClampValue(x.r + y.r),
                ClampValue(x.g + y.g),
                ClampValue(x.b + y.b));
        }
        public static Pixel operator -(Pixel x, Pixel y)
        {
            return new Pixel(
                ClampValue(x.r - y.r),
                ClampValue(x.g - y.g),
                ClampValue(x.b - y.b));
        }
        public static Pixel operator *(Pixel x, double y)
        {
            return new Pixel(
                ClampValue((int)(x.r * y)),
                ClampValue((int)(x.g * y)),
                ClampValue((int)(x.b * y)),
                x.a);
        }
        public static Pixel operator *(Pixel x, Pixel y)
        {
            return new Pixel(
                ClampValue((int)(x.r * y.r)),
                ClampValue((int)(x.g * y.r)),
                ClampValue((int)(x.b * y.r)),
                x.a);
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
    }
}
