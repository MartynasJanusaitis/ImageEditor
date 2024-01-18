using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoLooper
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
            this.r = (byte)r; 
            this.g = (byte)g; 
            this.b = (byte)b;
            a = 255;
        }
        public Pixel(byte r, byte g, byte b, byte a) : this(r, g, b)
        {
            this.a = a;
        }
        public Pixel(int r, int g, int b, int a) : this(r, g, b)
        {
            this.a = (byte)a;
        }
        public Pixel(Pixel pixel) : this(pixel.r, pixel.g, pixel.b, pixel.a)
        {

        }
        public static Pixel operator +(Pixel x, Pixel y)
        {
            int r = x.r + y.r;
            int g = x.g + y.g;
            int b = x.b + y.b;
            int a = x.a + y.a;
            if (r > 255) r = 255;
            if (g > 255) g = 255;
            if (b > 255) b = 255;
            if (a > 255) a = 255;
            return new Pixel(r, g, b);
        }
        public static Pixel operator -(Pixel x, Pixel y)
        {
            int r = x.r - y.r;
            int g = x.g - y.g;
            int b = x.b - y.b;
            int a = x.a - y.a;
            if (r < 0) r = 0;
            if (g < 0) g = 0;
            if (b < 0) b = 0;
            if (a < 0) a = 0;
            return new Pixel(r, g, b);
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
