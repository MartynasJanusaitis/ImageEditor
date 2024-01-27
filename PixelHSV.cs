using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    public class PixelHSV : IEquatable<PixelHSV>
    {
        public int h; // Hue 0-360
        public int s; // Saturation 0-100
        public int v; // Value 0-100
        public int a; // Alpha 0-255

        public PixelHSV(int h, int s, int v, int a = 255)
        {
            this.h = h;
            this.s = s;
            this.v = v;
            this.a = a;
        }
        public PixelHSV()
        {
            h = 0;
            s = 0;
            v = 0;
        }
        public PixelHSV(Pixel pixel)
        {
            double R = pixel.r / 255.0;
            double G = pixel.g / 255.0;
            double B = pixel.b / 255.0;

            double Cmax = Math.Max(Math.Max(R, G), B);
            double Cmin = Math.Min(Math.Min(R, G), B);

            double delta = Cmax - Cmin;

            if (delta == 0) h = 0;
            else if (Cmax == R) h = (int)Math.Round(60 * (((G - B) / delta) % 6));
            else if (Cmax == G) h = (int)Math.Round(60 * (((B - R) / delta) + 2));
            else h = (int)Math.Round(60 * (((R - G) / delta) + 4));

            if (Cmax == 0) s = 0;
            else s = (int)Math.Round((delta / Cmax) * 100);

            v = (int)Math.Round(Cmax * 100);

            a = pixel.a;
        }
        public bool Equals(PixelHSV other)
        {
            if (other is null) return false;

            return a == other.a &&
                h == other.h &&
                s == other.s &&
                v == other.v;
        }
        public override string ToString()
        {
            return $"H: {h}, S: {s}, V: {v}, A: {a}";
        }
    }
}
