using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ImageEditor
{
    internal class Image
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private Pixel[,] pixels;
        public Image(Pixel[,] pixel)
        {
            pixels = pixel;
            Width = pixel.GetLength(0);
            Height = pixel.GetLength(1);
        }
        public Image(int width, int height, Texture texture)
        {
            Width = width;
            Height = height;
            pixels = new Pixel[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixels[x, y] = texture.GetPixel(x, y);
                }
            }
        }
        public Image(int width, int height)
        {
            Width = width;
            Height = height;
            pixels = new Pixel[width, height];
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    pixels[x, y] = new Pixel();
                }
            }
        }
        public Image(Bitmap bitmap) : this(new Pixel[bitmap.Width, bitmap.Height])
        {
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                                   ImageLockMode.ReadWrite,
                                                   PixelFormat.Format32bppArgb);
            IntPtr ptr = bitmapData.Scan0;
            int bytesSize = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] bytes = new byte[bytesSize];
            Marshal.Copy(ptr, bytes, 0, bytesSize);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    int index = (y * bitmap.Width + x) * 4;
                    byte r = bytes[index];
                    byte g = bytes[index + 1];
                    byte b = bytes[index + 2];
                    byte a = bytes[index + 3];
                    pixels[x, y] = new Pixel(r, g, b, a);
                }
            }
        }
        public Image(string filename) : this(new Bitmap(filename)) { }
        public Pixel GetPixel(int x, int y)
        {
            if(x < 0 || y < 0) return null;
            if(x >= Width || y >= Height) return null;
            return pixels[x, y];
        }
        public Pixel[] GetCol(int x)
        {
            Pixel[] output = new Pixel[Height];
            for(int y  = 0; y < output.Length; y++)
            {
                output[y] = new Pixel(pixels[x, y]);
            }
            return output;
        }
        public Pixel[] GetRow(int y)
        {
            Pixel[] output = new Pixel[Width];
            for (int x = 0; x < output.Length; x++)
            {
                output[x] = new Pixel(pixels[x, y]);
            }
            return output;
        }
        public void PutPixel(int x, int y, Pixel value)
        {
            if (x >= Width || y >= Height) return;
            if (x < 0 || y < 0) return;
            pixels[x, y] = value;
        }
        public void SaveImage(string filename)
        {
            byte[] bytes = new byte[Width * Height * 4];
            for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height ; y++)
                {
                    int index = (y * Width + x) * 4;
                    Pixel pixel = pixels[x,y];
                    bytes[index] = pixel.b;
                    bytes[index + 1] = pixel.g;
                    bytes[index + 2] = pixel.r;
                    bytes[index + 3] = pixel.a;
                }
            }

            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            BitmapData bmpData = bitmap.LockBits(
                                 new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                 ImageLockMode.WriteOnly, bitmap.PixelFormat);

            Marshal.Copy(bytes, 0, bmpData.Scan0, bytes.Length);

            bitmap.UnlockBits(bmpData);
            bitmap.Save(filename);
        }
        public static Image operator +(Image image1, Image image2)
        {
            Image outputImage = new Image(
                Math.Max(image1.Width, image2.Width),
                Math.Max(image1.Height, image2.Height));

            for(int x = 0; x < outputImage.Width; x++)
            {
                for (int y = 0; y < outputImage.Height; y++)
                {
                    outputImage.PutPixel(x, y,
                        image1.GetPixel(x, y) + image2.GetPixel(x, y));
                }
            }

            return outputImage;
        }
    }
}
