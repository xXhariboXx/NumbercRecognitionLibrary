using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace WKiRO.NeuralNetwork.FeaturesExtraction
{
    using WKiRO.NeuralNetwork.Helpers;
    /// <summary>
    /// Contains logic for Barr Features  extraction.
    /// </summary>
    public class BarrFeatures : IFeatureExtractor<List<double>>
    {
        private Bitmap sourceBitmap;
        private Color[,] eastArray;
        private Color[,] northEastArray;
        private Color[,] northArray;
        private Color[,] northWestArray;

        /// <summary>
        /// Initialize object and sets all fields
        /// </summary>
        /// <param name="image">Image to extract features from</param> 
        public BarrFeatures(Bitmap image)
        {
            InitializeFields(image);
        }

        /// <summary>
        /// Method to initialize fields in object
        /// </summary>
        /// <param name="image">Image to extract features from</param> 
        private void InitializeFields(Bitmap image)
        {
            sourceBitmap = image;

            eastArray = new Color[image.Width, image.Height];
            northEastArray = new Color[image.Width, image.Height];
            northArray = new Color[image.Width, image.Height];
            northWestArray = new Color[image.Width, image.Height];
        }

        /// <summary>
        /// Method to extract Barr features from the image
        /// </summary>
        /// <param name="image">Image to extract features from</param>
        /// <returns>List of barr features from the given image</returns>
        public List<double> ExtractFeatures(Bitmap image)
        {
            double[] tempResultVector = new double[60];
            List<double> resultVector = new List<double>();
            InitializeFields(image);

            CreateDirectionalPictures();

            CalculateBarrFeatures(eastArray, sourceBitmap.Height, image.Width).CopyTo(tempResultVector, 0);
            CalculateBarrFeatures(northEastArray, sourceBitmap.Height, image.Width).CopyTo(tempResultVector, 15);
            CalculateBarrFeatures(northArray, sourceBitmap.Height, image.Width).CopyTo(tempResultVector, 30);
            CalculateBarrFeatures(northWestArray, sourceBitmap.Height, image.Width).CopyTo(tempResultVector, 45);

            resultVector = tempResultVector.ToList();
            return resultVector;
        }

        /// <summary>
        /// Creates directional images as a step of Barr features extraction process
        /// </summary>
        private void CreateDirectionalPictures()
        {
            var imageEast = new Bitmap(sourceBitmap);
            var imageNorthEast = new Bitmap(sourceBitmap);
            var imageNorth = new Bitmap(sourceBitmap);
            var imageNorthWest = new Bitmap(sourceBitmap);

            int whitizationFactor = 9;

            for (int i = 0; i < sourceBitmap.Height - 1; i++)
            {
                for (int j = 0; j < sourceBitmap.Width - 1; j++)
                {
                    eastArray[j, i] = imageEast.GetPixel(j, i);
                    northEastArray[j, i] = imageNorthEast.GetPixel(j, i);
                    northArray[j, i] = imageNorth.GetPixel(j, i);
                    northWestArray[j, i] = imageNorthWest.GetPixel(j, i);
                }
            }

            for (int i = 1; i < sourceBitmap.Height - 1; i++)
            {
                for (int j = 1; j < sourceBitmap.Width - 1; j++)
                {
                    int currentPixelGrey = ((eastArray[j, i].R + eastArray[j, i].G + eastArray[j, i].B) / 3);
                    if (currentPixelGrey <= 100)
                    {
                        continue;
                    }

                    int greyValueEast = ((eastArray[j - 1, i].R + eastArray[j - 1, i].G + eastArray[j - 1, i].B) / 3);
                    if (greyValueEast + whitizationFactor < 255)
                    {
                        greyValueEast += whitizationFactor;
                    }
                    eastArray[j, i] = Color.FromArgb(eastArray[j - 1, i].A, greyValueEast, greyValueEast, greyValueEast);

                    int greyValueNorthEast = ((northEastArray[j + 1, i - 1].R + northEastArray[j + 1, i - 1].G + northEastArray[j + 1, i - 1].B) / 3);
                    if (greyValueNorthEast + whitizationFactor < 255)
                    {
                        greyValueNorthEast += whitizationFactor;
                    }
                    northEastArray[j, i] = Color.FromArgb(northEastArray[j + 1, i - 1].A, greyValueNorthEast, greyValueNorthEast, greyValueNorthEast);

                    int greyValueNorth = ((northArray[j, i - 1].R + northArray[j, i - 1].G + northArray[j, i - 1].B) / 3);
                    if (greyValueNorth + whitizationFactor < 255)
                    {
                        greyValueNorth += whitizationFactor;
                    }
                    northArray[j, i] = Color.FromArgb(northArray[j, i - 1].A, greyValueNorth, greyValueNorth, greyValueNorth);

                    int colorNorthWest = ((northWestArray[j - 1, i - 1].R + northWestArray[j - 1, i - 1].G + northWestArray[j - 1, i - 1].B) / 3);
                    if (colorNorthWest + whitizationFactor < 255)
                    {
                        colorNorthWest += whitizationFactor;
                    }
                    northWestArray[j, i] = Color.FromArgb(northWestArray[j - 1, i - 1].A, colorNorthWest, colorNorthWest, colorNorthWest);
                }
            }

            for (int i = sourceBitmap.Height - 2; i > 0; i--)
            {
                for (int j = sourceBitmap.Width - 2; j > 0; j--)
                {
                    int currentPixelGrey = ((eastArray[j, i].R + eastArray[j, i].G + eastArray[j, i].B) / 3);
                    if (currentPixelGrey < 5)
                    {
                        continue;
                    }

                    //East
                    Color east1 = eastArray[j, i];
                    int eastGrey1 = (east1.R + east1.G + east1.B) / 3;
                    Color east2 = eastArray[j + 1, i];
                    int eastGrey2 = (east2.R + east2.G + east2.B) / 3;
                    int greyValueEast = Math.Max(eastGrey1, eastGrey2);
                    eastArray[j, i] = Color.FromArgb(east1.A, greyValueEast, greyValueEast, greyValueEast);

                    //NorthEast
                    Color northEast1 = northEastArray[j, i];
                    int northEastGrey1 = (northEast1.R + northEast1.G + northEast1.B) / 3;
                    Color northEast2 = northEastArray[j - 1, i + 1];
                    int northEastGrey2 = (northEast2.R + northEast2.G + northEast2.B) / 3;
                    int greyValueNorthEast = Math.Max(northEastGrey1, northEastGrey2);
                    northEastArray[j, i] = Color.FromArgb(northEast1.A, greyValueNorthEast, greyValueNorthEast, greyValueNorthEast);

                    //North
                    Color north1 = northArray[j, i];
                    int northGrey1 = (north1.R + north1.G + north1.B) / 3;
                    Color north2 = northArray[j, i + 1];
                    int northGrey2 = (north2.R + north2.G + north2.B) / 3;
                    int greyValueNorth = Math.Max(northGrey1, northGrey2);
                    northArray[j, i] = Color.FromArgb(north1.A, greyValueNorth, greyValueNorth, greyValueNorth);

                    //NorthWest
                    Color northWest1 = northWestArray[j, i];
                    int northWestgrey1 = (northWest1.R + northWest1.G + northWest1.B) / 3;
                    Color northWest2 = northWestArray[j + 1, i + 1];
                    int northWestGrey2 = (northWest2.R + northWest2.G + northWest2.B) / 3;
                    int greyValueNorthWest = Math.Max(northWestgrey1, northWestGrey2);
                    northWestArray[j, i] = Color.FromArgb(northWest1.A, greyValueNorthWest, greyValueNorthWest, greyValueNorthWest);
                }
            }

            for (int i = 0; i < sourceBitmap.Height - 1; i++)
            {
                for (int j = 0; j < sourceBitmap.Width - 1; j++)
                {
                    imageEast.SetPixel(i, j, eastArray[i, j]);
                    imageNorthEast.SetPixel(i, j, northEastArray[i, j]);
                    imageNorth.SetPixel(i, j, northArray[i, j]);
                    imageNorthWest.SetPixel(i, j, northWestArray[i, j]);
                }
            }

            imageEast.Save("East.bmp");
            imageNorthEast.Save("NorthEast.bmp");
            imageNorth.Save("North.bmp");
            imageNorthWest.Save("NorthWest.bmp");
        }

        /// <summary>
        /// Calculates Barr features from directional images
        /// </summary>
        /// <param name="imageGrayArray">Directional image in grayscale</param>
        /// <param name="height">Height of the image</param>
        /// <param name="width">Width of the image</param>
        /// <returns>Returns an array of Barr features from the directional image</returns>
        private double[] CalculateBarrFeatures(Color[,] imageGrayArray, int height, int width)
        {
            double[] resultVector = new double[15];

            int resultIndex = 0;

            int y = height / 3;
            int x = width / 2;

            for (int i = 0; i <= 4; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    double sum = 0;

                    for (int k = 0; k < y; k++)
                    {
                        for (int l = 0; l < x; l++)
                        {
                            sum += (double)imageGrayArray[((j * width) / 4) + l, ((i * height) / 6) + k].GetGrayScale();
                        }
                    }

                    resultVector[resultIndex] = sum / 60;
                    resultIndex++;
                }
            }

            return resultVector;
        }
    }
}
