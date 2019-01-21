/// <summary>
/// Namespace with features extracion methods
/// </summary>
namespace NumbersRecognitionLibrary.FeaturesExtraction
{
    using System.Collections.Generic;
    using System.Drawing;
    using NumbersRecognitionLibrary.Helpers;

    /// <summary>
    /// Contains logic for Distribution Sequence feature extraction.
    /// </summary>
    public class DistributionSequenceFeatureExtractor : IFeatureExtractor<List<double>>
    {
        /// <summary>
        /// Width and size of zone.
        /// </summary>
        public int ZoneSize { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DistributionSequenceFeatureExtractor()
        {
        }

        /// <summary>
        /// Initialize object with width and height of each zone of processed image.
        /// </summary>
        /// <param name="zoneSize">Width and height of zone.</param>
        public DistributionSequenceFeatureExtractor(int zoneSize)
        {
            ZoneSize = zoneSize;
        }

        #region IFeatureExtractor methods
        /// <summary>
        /// Extracts features following Distribution Sequence algorithm.
        /// </summary>
        /// <param name="image">Image to extract features from</param>
        /// <returns>List of features from the given image</returns>
        public List<double> ExtractFeatures(Bitmap bitmap)
        {
            int horizontalZonesCount = bitmap.Width / ZoneSize;
            int verticalZonesCount = bitmap.Height / ZoneSize;



            List<double> distributions = new List<double>();

            for (int x = 0; x < verticalZonesCount; x++)
            {
                for (int y = 0; y < horizontalZonesCount; y++)
                {
                    int zoneX = x * ZoneSize;
                    int zoneXLimit = (x + 1) * ZoneSize;
                    int objectPixelsCount = 0;


                    for (zoneX = zoneX; zoneX < zoneXLimit; zoneX++)
                    {
                        int zoneY = y * ZoneSize;
                        int zoneYLimit = (y + 1) * ZoneSize;

                        for (zoneY = zoneY; zoneY < zoneYLimit; zoneY++)
                        {
                            Color pixel = bitmap.GetPixel(zoneX, zoneY);
                            if (ExtensionMethods.AreEqual(pixel, Color.White))
                            {
                                objectPixelsCount++;
                            }
                        }
                    }

                    distributions.Add((float)objectPixelsCount / (ZoneSize * ZoneSize));
                }
            }

            return distributions;
        }
        #endregion
    }
}
