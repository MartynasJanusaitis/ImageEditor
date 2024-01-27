using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    internal static class TextureGenerator
    {
        public static Texture SolidColor(int r, int g, int b)
        {
            return new Texture((x, y) => new Pixel(r, g, b));
        }
        public static Texture Checkered(Pixel colorA, Pixel colorB, int size)
        {
            return new Texture((x, y) => (x / size % 2 == 0 ^ y / size % 2 == 0) ? colorB : colorA);
        }

        public static Texture RandomNoise(int minVal = 0, int maxVal = 255)
        {
            Random random = new Random();
            return new Texture((x, y) => new Pixel(
                random.Next(minVal, maxVal),
                random.Next(minVal, maxVal),
                random.Next(minVal, maxVal)));
        }
    }
}
