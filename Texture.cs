using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoLooper
{
    internal class Texture
    {
        private Func<int, int, Pixel> Generator;
        public Texture(Func<int, int, Pixel> generator)
        {
            Generator = generator;
        }

        public Pixel GetPixel(int x, int y)
        {
            return Generator(x, y);
        }
    }
}
