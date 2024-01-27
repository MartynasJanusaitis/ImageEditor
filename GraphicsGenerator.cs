using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    public static class GraphicsGenerator
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

        public static Image Circle(Image image, int x, int y, int radius, Texture texture)
        {
            for(int xi = x - radius; xi < x + radius; xi++)
            {
                for(int yi = y - radius; yi < y + radius; yi++)
                {
                    if(MathUtils.Distance(x, y, xi, yi) <= radius)
                    {
                        image.PutPixel(xi, yi, texture.GetPixel(xi, yi));
                    }
                }
            }

            return image;
        }
    }
}
