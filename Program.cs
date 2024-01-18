using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoLooper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Image image = new Image("Input.png");
            image = ImageProcessor.BoxBlur(image, 1);
            image.SaveImage("Output.png");

        }
    }
}
