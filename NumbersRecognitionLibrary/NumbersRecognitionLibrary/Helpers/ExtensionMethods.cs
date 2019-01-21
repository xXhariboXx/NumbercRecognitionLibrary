/// <summary>
/// Namespace with helper classes
/// </summary>
namespace NumbersRecognitionLibrary.Helpers
{
    using System.Drawing;
    /// <summary>
    /// Static class which contains logic for helpers methods
    /// </summary>
    public static class ExtensionMethods
    {
        #region Public methods
        /// <summary>
        /// Converts color to it's grayscale 
        /// </summary>
        /// <param name="color">Color to convert</param>
        /// <returns>Grayscale value of the color. Grayscale is represented as a float from range: 0-255</returns>
        public static float GetGrayScale(this Color color)
        {
            return (float)(color.R + color.G + color.B) / (3 * 255);
        }
        /// <summary>
        /// Compares colors
        /// </summary>
        /// <param name="color1">First color to compare</param>
        /// <param name="color2">Second color to compare</param>
        /// <returns>True if colors are the same</returns>
        public static bool AreEqual(Color color1, Color color2)
        {
            return color1.A == color2.A
                && color1.R == color2.R
                && color1.G == color2.G
                && color1.B == color2.B;
        }
        #endregion
    }
}
