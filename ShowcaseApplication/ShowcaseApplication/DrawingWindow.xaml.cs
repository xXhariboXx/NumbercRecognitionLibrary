using Microsoft.Win32;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NumbersRecognitionLibrary.NeuralNetwork;
using NumbersRecognitionLibrary.Helpers;

namespace WKiRO.MainApplication
{
    /// <summary>
    /// Interaction logic for DrawingWindow.xaml
    /// </summary>
    public partial class DrawingWindow : Window
    {
        NeuralNetworkType networkTypeFromView;
        NeuralNetworkOperator neuralNetworkOperator;
        double roundingFactor = 0.7;

        public DrawingWindow()
        {
            InitializeComponent();
            configManager = new ConfigManager();
            configManager.ReadNumbersConfigFile();
            configManager.isTesting = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            configManager.isTesting = true;
            int width = (int)canvas.Width;
            int height = (int)canvas.Height;

            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Default);
            rtb.Render(canvas);
            //save the ink to a memory stream
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            Bitmap bitmap;
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                bitmap = new Bitmap(ms);
                bitmap = ImagePreProcessing.ResizeImage(bitmap, 64, 64);
                bitmap = ImagePreProcessing.MedianFilter(bitmap);
                bitmap = ImagePreProcessing.NormalizePixels(bitmap, 0.3F);
            }

            NeuralNetworkType networkType;

            if (sondeApplicationRadioButton.IsChecked.HasValue && sondeApplicationRadioButton.IsChecked.Value)
            {
                networkType = NeuralNetworkType.Sondes;
            }
            else if (distributionSequenceRadioButton.IsChecked.HasValue && distributionSequenceRadioButton.IsChecked.Value)
            {
                networkType = NeuralNetworkType.DistributionSequence;
            }
            else
            {
                networkType = NeuralNetworkType.BarrFeatures;
            }

            configManager.SetInputNeuronAmountFromView(networkType);

            const double roundingFactor = 0.7F;
            var neuralNetworkOperator = new NeuralNetworkOperator(0.7F, configManager.InputNeuronAmount);
            neuralNetworkOperator.SetWeights(configManager.ReadWeights());
            double[] features = configManager.GetImageFeatureVector(bitmap);
            double[] result = neuralNetworkOperator.RecognizeNumber(features);

            foreach (var resultValue in result)
            {

            }
            if (result.Length != 0)
            {
                
                MessageBox.Show(GetNumberName(result, configManager.NumberList));
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

            //numberResult += "\nResult: ";
            //foreach (double resultBit in outputVector)
            //{
            //    numberResult += "\n" + resultBit.ToString("N3");
            //}

            return answer;
        }


        private ConfigManager configManager;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            configManager.isTesting = true;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            double[] results = null;
            if (openFileDialog.ShowDialog() == true)
            {
                var image = System.Drawing.Image.FromFile(openFileDialog.InitialDirectory + openFileDialog.FileName);
                var resized = ImagePreProcessing.ResizeImage(image, 64, 64);
                var medianized = ImagePreProcessing.MedianFilter(resized);
                var normalized = ImagePreProcessing.NormalizePixels(medianized, 0.3F);

                NeuralNetworkType networkType;

                if (sondeApplicationRadioButton.IsChecked.HasValue && sondeApplicationRadioButton.IsChecked.Value)
                {
                    networkType = NeuralNetworkType.Sondes;
                }
                else if (distributionSequenceRadioButton.IsChecked.HasValue && distributionSequenceRadioButton.IsChecked.Value)
                {
                    networkType = NeuralNetworkType.DistributionSequence;
                }
                else
                {
                    networkType = NeuralNetworkType.BarrFeatures;
                }

                configManager.SetInputNeuronAmountFromView(networkType);

                const double roundingFactor = 0.7F;
                var neuralNetworkOperator = new NeuralNetworkOperator(0.7F, configManager.InputNeuronAmount);
                neuralNetworkOperator.SetWeights(configManager.ReadWeights());

                double[] features = configManager.GetImageFeatureVector(normalized);
                results = neuralNetworkOperator.RecognizeNumber(features);

                if (results.Length != 0)
                {
                    MessageBox.Show(GetNumberName(results, configManager.NumberList));
                }
            }
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

        private void TrainButton_Click(object sender, RoutedEventArgs e)
        {
            configManager.isTesting = false;
            configManager.MainList.Clear();
            configManager.SetInputNeuronAmountFromView(networkTypeFromView);
            configManager.ReadImagesFromFile(configManager.TrainDataFolderPath);
            //configManager.SetCurrentNeuralNetworkType(configManager.neuralNetworkType);
            neuralNetworkOperator = new NeuralNetworkOperator(roundingFactor, configManager.InputNeuronAmount);
            //configManager.NeuralNetworkOperator = neuralNetworkOperator;
            //var testBitmap = Image.FromFile("domoTest.jpg");




            //fileReader.CreateListOfArrays(fileReader.TrainDataFolderPath);
            //ResultTextBox.Text = fileReader.PrintListOfArrays(fileReader.MainList);
            neuralNetworkOperator.Train(configManager.MainList);
            configManager.CanSaveNetwork = true;
            //SaveNetworkButton.IsEnabled = true;
            TestButton.IsEnabled = true;
            //OpenFileButton.IsEnabled = true;
            if (distributionSequenceRadioButton.IsEnabled || barrFeaturesRadioButton.IsEnabled || applicationOfTheSondesRadioButton.IsEnabled)
            {
                this.SaveNetworkButton.IsEnabled = true;
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            configManager.isTesting = true;
            configManager.MainList.Clear();
            configManager.ReadImagesFromFile(configManager.TestDataFolderPath);
            ResultTextBox.Clear();
            ResultTextBox.Text = neuralNetworkOperator.Test(configManager.MainList);
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


            //OpenFileButton.IsEnabled = true;
        }

        private void SaveNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            configManager.SaveWeights(neuralNetworkOperator.Weights);
        }
    }
}
