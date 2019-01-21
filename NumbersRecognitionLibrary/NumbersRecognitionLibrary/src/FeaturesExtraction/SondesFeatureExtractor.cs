namespace WKiRO.NeuralNetwork.FeaturesExtraction
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using WKiRO.NeuralNetwork.Helpers;

    /// <summary>
    /// Contains Application of Sondes algorithm feature extraction.
    /// </summary>
    public class SondesFeatureExtractor : IFeatureExtractor<List<double>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SondesFeatureExtractor()
        {
        }
        /// <summary>
        /// Initializes Sondes Extractor with offset of consecutive sondes in all directions.
        /// <param name="image">Image to extract features from</param>
        /// <returns>List of features from the given image</returns>
        public SondesFeatureExtractor(int sondeOffset)
        {
            SondeOffset = sondeOffset;
        }

        /// <summary>
        /// Represents a number of pixels skipped before applying a sonde.
        /// </summary>
        public int SondeOffset { get; set; }

        /// <summary>
        /// Extracts Application of Sondes features.
        /// </summary>
        /// <param name="bitmap">Image to extract features from.</param>
        /// <returns>List of features for sondes application algorithm in left-top-right-bottom manner.</returns>
        public List<double> ExtractFeatures(Bitmap bitmap)
        {
            return ApplyLeftSondes(bitmap)
                            .Concat(ApplyTopSondes(bitmap))
                            .Concat(ApplyRightSondes(bitmap))
                            .Concat(ApplyBottomSondes(bitmap))
                            .ToList();
        }

        /// <summary>
        /// Applies sondes for left border.
        /// </summary>
        /// <param name="bitmap">Image object.</param>
        /// <returns>List of features for left border of <paramref name="bitmap"/></returns>
        private List<double> ApplyLeftSondes(Bitmap bitmap)
        {
            List<double> leftSondeValues = new List<double>();

            for (int y = 0; y < bitmap.Height; y += SondeOffset)
            {
                int x = 0;
                bool foundObjectPixel = false;
                while (x < bitmap.Width && foundObjectPixel == false)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);

                    if (ExtensionMethods.AreEqual(pixelColor, Color.White))
                    {
                        double normalizedLength = (double)x / bitmap.Width;
                        leftSondeValues.Add(normalizedLength);
                        foundObjectPixel = true;
                    }
                    else
                    {
                        x++;
                    }
                }

                if (foundObjectPixel == false)
                {
                    leftSondeValues.Add(1.0f);
                }
            }

            return leftSondeValues;
        }

        /// <summary>
        /// Applies sondes for top border.
        /// </summary>
        /// <param name="bitmap">Image object.</param>
        /// <returns>List of features for top border of <paramref name="bitmap"/>.</returns>
        private List<double> ApplyTopSondes(Bitmap bitmap)
        {
            List<double> topSondeValues = new List<double>();

            for (int x = 0; x < bitmap.Width; x += SondeOffset)
            {
                int y = 0;
                bool foundObjectPixel = false;

                while (y < bitmap.Height && foundObjectPixel == false)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);

                    if (ExtensionMethods.AreEqual(pixelColor, Color.White))
                    {
                        double normalizedLength = (double)y / bitmap.Height;
                        topSondeValues.Add(normalizedLength);
                        foundObjectPixel = true;
                    }
                    else
                    {
                        y++;
                    }
                }

                if (foundObjectPixel == false)
                {
                    topSondeValues.Add(1.0F);
                }
            }

            return topSondeValues;
        }

        /// <summary>
        /// Applies sondes for right border.
        /// </summary>
        /// <param name="bitmap">Image object.</param>
        /// <returns>List of features for right border of <paramref name="bitmap"/></returns>
        private List<double> ApplyRightSondes(Bitmap bitmap)
        {
            List<double> rightSondeValues = new List<double>();

            for (int y = 0; y < bitmap.Height; y += SondeOffset)
            {
                int x = bitmap.Width - 1;
                bool foundObjectPixel = false;

                while (x >= 0 && foundObjectPixel == false)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);

                    if (ExtensionMethods.AreEqual(pixelColor, Color.White))
                    {
                        double normalizedLength = (double)x / bitmap.Width;
                        rightSondeValues.Add(normalizedLength);
                        foundObjectPixel = true;
                    }
                    else
                    {
                        x--;
                    }
                }

                if (foundObjectPixel == false)
                {
                    rightSondeValues.Add(0.0F);
                }
            }

            return rightSondeValues;
        }

        /// <summary>
        /// Applies sondes for bottom border.
        /// </summary>
        /// <param name="bitmap">Image object.</param>
        /// <returns>List of features for bottom border of <paramref name="bitmap"/></returns>
        private List<double> ApplyBottomSondes(Bitmap bitmap)
        {
            List<double> bottomSondeValues = new List<double>();

            for (int x = 0; x < bitmap.Width; x += SondeOffset)
            {
                int y = bitmap.Height - 1;
                bool foundObjectPixel = false;

                while (y >= 0 && foundObjectPixel == false)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);

                    if (ExtensionMethods.AreEqual(pixelColor, Color.White))
                    {
                        double normalizedLength = (double)y / bitmap.Height;
                        bottomSondeValues.Add(normalizedLength);
                        foundObjectPixel = true;
                    }
                    else
                    {
                        y--;
                    }
                }

                if (foundObjectPixel == false)
                {
                    bottomSondeValues.Add(0.0F);
                }
            }

            return bottomSondeValues;
        }
    }
}
