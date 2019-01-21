namespace WKiRO.NeuralNetwork.Helpers
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Drawing.Drawing2D;
    using System.Collections.Generic;

    public class ImagePreProcessing
    {
        /// <summary>
        /// Returns resized image specified in <paramref name="image"/> parameter.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="width">Target width.</param>
        /// <param name="height">Target height.</param>
        /// <returns>Resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Returns median value
        /// </summary>
        /// <param name="collection">Value to find median value</param>
        /// <returns>Median value</returns>
        private static float Median(List<float> collection)
        {
            collection.Sort();
            int index = collection.Count / 2;

            if (collection.Count % 2 == 0)
            {
                return (collection[index] + collection[index + 1]) / 2;
            }
            else
            {
                return collection[index];
            }
        }
        /// <summary>
        /// Creates image with median of nearest pixels
        /// </summary>
        /// <param name="bitmap">Imege to process</param>
        /// <returns>Median image of the input image</returns>
        public static Bitmap MedianFilter(Bitmap bitmap)
        {
            Bitmap outputBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color middleColor = bitmap.GetPixel(x, y);
                    List<float> neighbourColorList = new List<float>();

                    if (x == 0 && y == 0)
                    {
                        neighbourColorList.Add(bitmap.GetPixel(1, 0).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(1, 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(0, 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(0, 0).GetGrayScale());
                    }

                    else if (x == bitmap.Width - 1 && y == 0)
                    {
                        int tempX = bitmap.Width - 1;
                        neighbourColorList.Add(bitmap.GetPixel(tempX, 0).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(tempX - 1, 0).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(tempX - 1, 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(tempX, 1).GetGrayScale());
                    }

                    else if (x == bitmap.Width - 1 && y == bitmap.Height - 1)
                    {
                        int tempX = bitmap.Width - 1;
                        int tempY = bitmap.Height - 1;

                        neighbourColorList.Add(bitmap.GetPixel(tempX, tempY).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(tempX, tempY - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(tempX - 1, tempY - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(tempX - 1, tempY).GetGrayScale());
                    }
                    else if (x == 0 && y == bitmap.Height - 1)
                    {
                        int tempY = bitmap.Height - 1;

                        neighbourColorList.Add(bitmap.GetPixel(0, tempY).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(0, tempY - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(1, tempY - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(1, tempY).GetGrayScale());
                    }
                    else if (x == 0)
                    {
                        neighbourColorList.Add(bitmap.GetPixel(0, y).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(0, y - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(1, y - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(1, y).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(1, y + 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(0, y + 1).GetGrayScale());
                    }
                    else if (x == bitmap.Width - 1)
                    {
                        int tempX = bitmap.Width - 1;
                        neighbourColorList.Add(bitmap.GetPixel(tempX, y).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(tempX, y - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(tempX - 1, y - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(tempX - 1, y).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(tempX - 1, y + 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(tempX, y + 1).GetGrayScale());
                    }
                    else if (y == 0)
                    {
                        neighbourColorList.Add(bitmap.GetPixel(x, 0).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x - 1, 0).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x - 1, 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x, 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x + 1, 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x + 1, 0).GetGrayScale());
                    }
                    else if (y == bitmap.Height - 1)
                    {
                        int tempY = bitmap.Height - 1;

                        neighbourColorList.Add(bitmap.GetPixel(x, tempY).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x - 1, tempY).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x - 1, tempY - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x, tempY - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x + 1, tempY - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x + 1, tempY).GetGrayScale());
                    }

                    else
                    {
                        neighbourColorList.Add(bitmap.GetPixel(x, y).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x - 1, y - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x, y - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x + 1, y - 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x + 1, y).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x + 1, y + 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x, y + 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x - 1, y + 1).GetGrayScale());
                        neighbourColorList.Add(bitmap.GetPixel(x - 1, y).GetGrayScale());
                    }

                    float median = Median(neighbourColorList);
                    int rgbMedian = (int)(median * 255);

                    outputBitmap.SetPixel(x, y, Color.FromArgb(255, rgbMedian, rgbMedian, rgbMedian));

                }
            }
            return outputBitmap;
        }

        /// <summary>
        /// Obtains table of normalized pixel values of the image. The discrimination depends on <paramref name="grayscaleFactor"/> - pixels having higher grayscale than <paramref name="grayscaleFactor"/> are recognized as object pixels, else - background pixels.
        /// </summary>
        /// <param name="bitmap">Input image.</param>
        /// <param name="grayscaleFactor">Discrimination factor.</param>
        /// <returns>Table of normalized pixels value.</returns>
        public static Bitmap NormalizePixels(Bitmap bitmap, float grayscaleFactor)
        {
            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    if (color.GetGrayScale() > grayscaleFactor)
                    {
                        result.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        result.SetPixel(x, y, Color.Black);
                    }
                }
            }

            return result;
        }
    }
}
