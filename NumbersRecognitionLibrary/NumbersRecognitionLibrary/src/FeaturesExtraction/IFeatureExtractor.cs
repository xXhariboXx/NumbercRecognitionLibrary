namespace WKiRO.NeuralNetwork.FeaturesExtraction
{
    using System.Drawing;
    /// <summary>
    /// Interface for feature extractor classes
    /// </summary>
    /// <typeparam name="T">Type of return features data</typeparam>
    public interface IFeatureExtractor<T>
    {
        /// <summary>
        /// Method to extract features from the image
        /// </summary>
        /// <param name="bitmap">Image to extract features from</param>
        /// <returns>Features extracted from the image</returns>
        T ExtractFeatures(Bitmap bitmap);
    }
}
