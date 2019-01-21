using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using NumbersRecognitionLibrary.Helpers;
using NumbersRecognitionLibrary.FeaturesExtraction;

namespace WKiRO.MainApplication
{
    public enum NeuralNetworkType
    {
        None,
        DistributionSequence,
        Sondes,
        BarrFeatures
    }

    public struct Number
    {
        public int[] BitCode;
        public int Digit;

        public Number(int[] bitCode, int digit)
        {
            this.BitCode = bitCode;
            this.Digit = digit;
        }
    }

    public class ConfigManager
    {
        const String ConfigFileName = "ConfigFile.txt";
        const String TrainDataFolderName = "traindata";
        const String TestDataFolderName = "testdata";
        const String WeightsFolderName = "weights";
        const String DistributionSequenceFileName = "DistributionSequence.txt";
        const String SondesFileName = "Sondes.txt";
        const String BarrFeaturesFileName = "BarrFeatures.txt";

        String ConfigFilePath;

        public NeuralNetworkType currentNeuralNetworkType { get; set; }

        private StreamReader streamReader;
        private StreamWriter streamWriter;
        public List<Number> NumberList { get; }
        public List<double[]> MainList { get; }
        public String TrainDataFolderPath { get; }
        public String TestDataFolderPath { get; }
        public String WeightsFolderPath { get; }
        public bool isTesting { get; set; }
        public NeuralNetworkType neuralNetworkType { get; set; }
        public bool CanSaveNetwork { get; set; } = false;
        public int InputNeuronAmount { get; set; }

        string root = "..\\..\\data\\";

        public ConfigManager()
        {
            MainList = new List<double[]>();
            NumberList = new List<Number>();
            TrainDataFolderPath += root + TrainDataFolderName;
            ConfigFilePath += root + ConfigFileName;
            TestDataFolderPath += root + TestDataFolderName;
            WeightsFolderPath += root + WeightsFolderName;
            neuralNetworkType = NeuralNetworkType.None;
        }

        public void SetCurrentNeuralNetworkType(NeuralNetworkType type)
        {
            currentNeuralNetworkType = type;
        }

        public void SetInputNeuronAmountFromView(NeuralNetworkType type)
        {
            currentNeuralNetworkType = type;
            switch (type)
            {
                case NeuralNetworkType.DistributionSequence:
                    {
                        InputNeuronAmount = 64;
                        break;
                    }
                case NeuralNetworkType.Sondes:
                    {
                        InputNeuronAmount = 32;
                        break;
                    }
                case NeuralNetworkType.BarrFeatures:
                    {
                        InputNeuronAmount = 60;
                        break;
                    }
                default:
                    break;
            }
        }

        private List<double> ExtractFeatures(Bitmap normalizedBitmap, NeuralNetworkType neuralNetworkType)
        {
            switch (neuralNetworkType)
            {
                case NeuralNetworkType.DistributionSequence:
                    {
                        var extractor = new DistributionSequenceFeatureExtractor(8);
                        return extractor.ExtractFeatures(normalizedBitmap);
                    }
                case NeuralNetworkType.Sondes:
                    {
                        var extractor = new SondesFeatureExtractor(8);
                        return extractor.ExtractFeatures(normalizedBitmap);
                    }
                case NeuralNetworkType.BarrFeatures:
                    {
                        var extractor = new BarrFeatures(normalizedBitmap);
                        return extractor.ExtractFeatures(normalizedBitmap);
                        return null;
                    }
                default:
                    return null;
            }
        }

        public double[] GetImageFeatureVector(Bitmap bitmap)
        {
            var features = ExtractFeatures(bitmap, currentNeuralNetworkType);
            return features.ToArray();
        }

        public void ReadImagesFromFile(String path)
        {
            try
            {
                var numberFiles = Directory.EnumerateDirectories(path);
                foreach (String currentDir in numberFiles)
                {
                    var imageFiles = Directory.EnumerateFiles(currentDir, "*.jpg");
                    int sumOfSources = 0;
                    double[] languageAveragePercentageArray = new double[27];
                    foreach (String currentFile in imageFiles)
                    {
                        int maxTrainFilesAmount = isTesting ? 300 : 700;
                        if (sumOfSources > maxTrainFilesAmount)
                        {
                            break;
                        }
                        sumOfSources++;
                        String number = currentDir.Remove(0, currentDir.Length - 1);

                        var img = Image.FromFile(currentFile);
                        var testBitmap = (Bitmap)img;
                        var medianizedBitmap = ImagePreProcessing.MedianFilter(testBitmap);
                        var normalizedBitmap = ImagePreProcessing.NormalizePixels(medianizedBitmap, 0.3F);
                        var features = ExtractFeatures(normalizedBitmap, currentNeuralNetworkType);
                        ConvertNumberAndAddToVector(number, features);
                        MainList.Add(features.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("blad");
            }
        }

        //Converts language from name to bit code and inserts it to data vector
        private void ConvertNumberAndAddToVector(string numberName, List<double> featuresArray)
        {
            Int32.TryParse(numberName, out int digit);
            Number number = NumberList.First(d => d.Digit == digit);
            List<double> bitList = new List<double>();
            foreach (int bit in number.BitCode)
            {
                bitList.Add(bit);
            }
            featuresArray.AddRange(bitList);
        }

        //Reads weights from weights file
        public double[] ReadWeights()
        {
            string fileName = GetWeightFileName(currentNeuralNetworkType);
            string path = WeightsFolderPath + fileName;
            List<double> weightsList = new List<double>();

            using (streamReader = new StreamReader(path))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    weightsList.Add(Double.Parse(line));
                }
            }

            double[] result = new double[weightsList.Count];
            for (int i = 0; i < weightsList.Count; i++)
            {
                result[i] = weightsList.ElementAt(i);
            }
            return result;
        }

        //Saves weights from current neural network to file
        public void SaveWeights(double[] weights)
        {
            string fileName = GetWeightFileName(currentNeuralNetworkType);
            string path = WeightsFolderPath + fileName;
            using (streamWriter = new StreamWriter(path))
            {
                String weightsString = "";
                for (int i = 0; i < weights.Length; i++)
                {
                    streamWriter.WriteLine(weights[i]);
                }
            }
        }

        //Reads config file and creates data about supported languages
        public void ReadNumbersConfigFile()
        {
            using (streamReader = new StreamReader(ConfigFilePath))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    line = line.ToUpper();
                    Int32.TryParse(line.Substring(0, 1), out int number);
                    int[] numberBits = new int[10];
                    for (int i = 0; i < numberBits.Length; i++)
                    {
                        numberBits[i] = (int)Char.GetNumericValue(line.ElementAt(i + 2));
                    }
                    NumberList.Add(new Number(numberBits, number));
                }
            }
        }

        private string GetWeightFileName(NeuralNetworkType type)
        {
            string fileName = "\\";
            switch (type)
            {
                case NeuralNetworkType.DistributionSequence:
                    fileName += DistributionSequenceFileName;
                    break;
                case NeuralNetworkType.Sondes:
                    fileName += SondesFileName;
                    break;
                case NeuralNetworkType.BarrFeatures:
                    fileName += BarrFeaturesFileName;
                    break;
                default:
                    return "";
            }
            return fileName;
        }
    }
}
