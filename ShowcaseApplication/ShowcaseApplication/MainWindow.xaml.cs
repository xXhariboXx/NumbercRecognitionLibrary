using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using NumbersRecognitionLibrary.NeuralNetwork;
using NumbersRecognitionLibrary.Helpers;

namespace WKiRO.MainApplication
{
    public partial class MainWindow : Window
    {
        ConfigManager configManager;
        NeuralNetworkOperator neuralNetworkOperator;
        NeuralNetworkType networkTypeFromView;
        double roundingFactor = 0.7;

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            configManager = new ConfigManager();
            configManager.ReadNumbersConfigFile();
        }

        private void TrainButton_Click(object sender, RoutedEventArgs e)
        {
            configManager.MainList.Clear();
            configManager.SetInputNeuronAmountFromView(networkTypeFromView);
            configManager.ReadImagesFromFile(configManager.TrainDataFolderPath);
            neuralNetworkOperator = new NeuralNetworkOperator(roundingFactor, configManager.InputNeuronAmount);

            neuralNetworkOperator.Train(configManager.MainList);
            configManager.CanSaveNetwork = true;
            TestButton.IsEnabled = true;
            if (distributionSequenceRadioButton.IsEnabled || barrFeaturesRadioButton.IsEnabled || applicationOfTheSondesRadioButton.IsEnabled)
            {
                this.SaveNetworkButton.IsEnabled = true;
            }
        }

        private void SaveNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            configManager.SaveWeights(neuralNetworkOperator.Weights);
        }

        private void DisplayLanguages()
        {
            string languages = "";
        }

        private string ArrayToString(int[] array)
        {
            string result = "";
            for (int i = 0; i < array.Length; i++)
            {
                result += array[i];
            }
            return result;
        }

        private string ArrayToString(double[] array)
        {
            string result = "";
            for (int i = 0; i < array.Length; i++)
            {
                result += array[i];
            }
            return result;
        }

        private void LoadNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                configManager.SetInputNeuronAmountFromView(networkTypeFromView);
                neuralNetworkOperator = new NeuralNetworkOperator(roundingFactor, configManager.InputNeuronAmount);
                neuralNetworkOperator.SetWeights(configManager.ReadWeights());
                TestButton.IsEnabled = true;
            }
            catch
            {
                MessageBox.Show("There is no such file",
                                          "Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            configManager.MainList.Clear();
            configManager.ReadImagesFromFile(configManager.TestDataFolderPath);
            ResultTextBox.Clear();
            ResultTextBox.Text = neuralNetworkOperator.Test(configManager.MainList);
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            double[] results = null;
            if (openFileDialog.ShowDialog() == true)
            {
                var image = System.Drawing.Image.FromFile(openFileDialog.InitialDirectory + openFileDialog.FileName);
                var resized = ImagePreProcessing.ResizeImage(image, 64, 64);
                var medianized = ImagePreProcessing.MedianFilter(resized);
                var normalized = ImagePreProcessing.NormalizePixels(medianized, 0.3F);

                double[] features = configManager.GetImageFeatureVector(normalized);
                results = neuralNetworkOperator.RecognizeNumber(features);

                if (results.Length != 0)
                {
                    MessageBox.Show(GetNumberName(results, configManager.NumberList));
                }
            }
        }


        private string GetNumberName(double[] outputVector, List<Number> numberList)
        {
            int[] intResult = new int[outputVector.Length];
            string answer = "";
            int numberResult = new int();
            for (int i = 0; i < outputVector.Length; i++)
            {
                if (outputVector[i] >= 0.7)
                {
                    intResult[i] = 1;
                }
            }
            foreach (Number number in numberList)
            {
                if (number.BitCode.SequenceEqual(intResult))
                {
                    numberResult = number.Digit;
                    answer += numberResult.ToString();
                    break;
                }
            }

            if (numberResult == null)
            {
                answer = "Number not reconized";
            }

            return answer;
        }

        private void distributionSequenceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SetOptionsButtons();
            networkTypeFromView = NeuralNetworkType.DistributionSequence;
        }

        private void applicationOfTheSondesRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SetOptionsButtons();
            networkTypeFromView = NeuralNetworkType.Sondes;
        }

        private void barrFeaturesRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SetOptionsButtons();
            networkTypeFromView = NeuralNetworkType.BarrFeatures;
        }

        private void SetOptionsButtons()
        {
            this.LoadNetworkButton.IsEnabled = true;
            this.TrainButton.IsEnabled = true;
            if (configManager.CanSaveNetwork)
            {
                this.SaveNetworkButton.IsEnabled = true;
            }
        }
    }
}


