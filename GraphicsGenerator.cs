using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoLooper
{
    internal static class GraphicsGenerator
    {
        public static Image Rectangle(Image image, int x, int y, int width, int height, Texture texture)
        {
            for(int xi = x; xi < x + width; xi++)
            {
                for(int yi = y; yi < y + height; yi++)
                {
                    image.PutPixel(xi, yi, texture.GetPixel(xi, yi));
                }
            }

            return image;
        }
    }
}
