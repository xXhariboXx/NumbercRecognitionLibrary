
namespace WKiRO.NeuralNetwork.FeaturesExtraction
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class CenterMomentsOfDifferentProfilesExtractor : IFeatureExtractor<List<double>>
    {
        public List<double> ExtractFeatures(Bitmap bitmap)
        {
            List<int> leftProfiles = GetLeftProfiles(bitmap);

            //using (var writer = new StreamWriter("file.csv"))
            //{
            //    for (int i = 0; i < leftProfiles.Count; i++)
            //    {
            //        writer.Write(leftProfiles[i]);
            //        if(i != leftProfiles.Count - 1)
            //        {
            //            writer.Write(';');
            //        }
            //    }
            //}

            double average = leftProfiles.Average();

            List<double> momentsList = new List<double>();
            for (int centralMomentIndex = 1; centralMomentIndex <= 6; centralMomentIndex++)
            {
                double rawMomentValue = 0;
                for (int i = 0; i < leftProfiles.Count; i++)
                {
                    int x = leftProfiles[i];

                    int lessThanAverage = leftProfiles.Where(s => s < average).Count();


                    double result = x - average;

                    rawMomentValue += Math.Pow(result, centralMomentIndex);

                }

                rawMomentValue = rawMomentValue / (double)leftProfiles.Count;

                momentsList.Add(rawMomentValue);
            }

            return momentsList;
        }

        private List<int> GetLeftProfiles(Bitmap bitmap)
        {
            var leftProfiles = new List<int>();


            for (int i = 0; i < bitmap.Height; i++)
            {
                int width = 0;
                bool found = false;
                while (width < bitmap.Width && found == false)
                {
                    Color color = bitmap.GetPixel(width, i);
                    float grayscale = color.GetGrayScale();

                    if (grayscale > 0.3)
                    {
                        leftProfiles.Add(width);
                        found = true;
                    }

                    width++;
                }

                if (found == false)
                {
                    leftProfiles.Add(bitmap.Width - 1);
                }
            }

            return leftProfiles;
        }
    }
}
