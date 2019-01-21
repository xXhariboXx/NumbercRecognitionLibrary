/// <summary>
/// Namespace with neural network classes
/// </summary>
namespace NumbersRecognitionLibrary.NeuralNetwork
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// Class that operates neural network
    /// </summary>
    public class NeuralNetworkOperator
    {
        private int hiddenNeuronsAmount;
        private int outputNeuronsAmount;
        private NeuralNetwork neuralNetwork;
        /// <summary>
        /// Array of weights for the neural network
        /// </summary>
        public double[] Weights { get; set; }
        public double RoundingFactor { get; set; }
        /// <summary>
        /// Number of input vectors
        /// </summary>
        public int InputNeuronAmount { get; set; }

        public NeuralNetworkOperator(int inputNeuronAmount)
        {
            InputNeuronAmount = inputNeuronAmount;
            RoundingFactor = 0.8;
            SetupNetwork();
        }

        public NeuralNetworkOperator(double roundingFactor, int inputNeuronAmount)
        {
            InputNeuronAmount = inputNeuronAmount;
            RoundingFactor = roundingFactor;
            SetupNetwork();
        }

        #region Public methods
        /// <summary>
        /// Sets weights for the neural network
        /// </summary>
        /// <param name="weights">Weights to set in the neural network</param>
        public void SetWeights(double[] weights)
        {
            Weights = weights;
            neuralNetwork.SetWeights(Weights);
        }
        /// <summary>
        /// Trains the neural network
        /// </summary>
        /// <param name="inputVector">List of test data input vectors</param>
        public void Train(List<double[]> inputVector)
        {
            int numRows = inputVector.Count;
            int seed = 1;

            double[][] allData = MakeAllData(InputNeuronAmount, hiddenNeuronsAmount, outputNeuronsAmount,
              numRows, seed, inputVector);


            int maxEpochs = 1000;
            double learnRate = 0.05;
            double momentum = 0.01;

            Console.WriteLine("\nStarting training");
            Weights = neuralNetwork.Train(allData, maxEpochs, learnRate, momentum);
            Console.WriteLine("Done");
            Console.WriteLine("\nFinal neural network model weights and biases:\n");
            ShowVector(Weights, 2, 10, true);
        }

        public string Test(List<double[]> inputVector)
        {

            int numRows = inputVector.Count;
            int seed = 1; // gives nice demo

            double[][] allData = MakeAllData(InputNeuronAmount, hiddenNeuronsAmount, outputNeuronsAmount,
              numRows, seed, inputVector);


            double testAcc = neuralNetwork.Accuracy(allData);
            string result = "Final accuracy on test data     = " +
              testAcc.ToString("F4");
            return result;
        }
        /// <summary>
        /// Recognizes the number
        /// </summary>
        /// <param name="inputVector">Vector of input features</param>
        /// <returns>Recognized number</returns>
        public double[] RecognizeNumber(double[] inputVector)
        {
            return neuralNetwork.ComputeOutputs(inputVector);
        }
        #endregion

        #region Private methods
        private void SetupNetwork()
        {
            hiddenNeuronsAmount = 7;
            outputNeuronsAmount = 10;
            neuralNetwork = new NeuralNetwork(InputNeuronAmount, hiddenNeuronsAmount, outputNeuronsAmount, RoundingFactor);
        }
        private void ShowMatrix(double[][] matrix, int numRows,
     int decimals, bool indices)
        {
            int len = matrix.Length.ToString().Length;
            for (int i = 0; i < numRows; ++i)
            {
                if (indices == true)
                    Console.Write("[" + i.ToString().PadLeft(len) + "]  ");
                for (int j = 0; j < matrix[i].Length; ++j)
                {
                    double v = matrix[i][j];
                    if (v >= 0.0)
                        Console.Write(" "); // '+'
                    Console.Write(v.ToString("F" + decimals) + "  ");
                }
                Console.WriteLine("");
            }

            if (numRows < matrix.Length)
            {
                Console.WriteLine(". . .");
                int lastRow = matrix.Length - 1;
                if (indices == true)
                    Console.Write("[" + lastRow.ToString().PadLeft(len) + "]  ");
                for (int j = 0; j < matrix[lastRow].Length; ++j)
                {
                    double v = matrix[lastRow][j];
                    if (v >= 0.0)
                        Console.Write(" "); // '+'
                    Console.Write(v.ToString("F" + decimals) + "  ");
                }
            }
            Console.WriteLine("\n");
        }

        private void ShowVector(double[] vector, int decimals,
          int lineLen, bool newLine)
        {
            for (int i = 0; i < vector.Length; ++i)
            {
                if (i > 0 && i % lineLen == 0) Console.WriteLine("");
                if (vector[i] >= 0) Console.Write(" ");
                Console.Write(vector[i].ToString("F" + decimals) + " ");
            }
            if (newLine == true)
                Console.WriteLine("");
        }

        private double[][] MakeAllData(int numInput, int numHidden,
          int numOutput, int numRows, int seed, List<double[]> inputVector)
        {
            //alocating memory
            double[][] result = new double[numRows][]; // allocate return-result
            for (int i = 0; i < numRows; ++i)
                //alocating space for input+output
                result[i] = new double[numInput + numOutput]; // 1-of-N in last column
            //end of allocating

            for (int i = 0; i < inputVector.Count; i++)
            {
                for (int j = 0; j < (numInput + numOutput); j++)
                {
                    result[i][j] = inputVector.ElementAt(i)[j];
                }
            }

            return result;
        } // MakeAllData

        private void SplitTrainTest(double[][] allData, double trainPct,
          int seed, out double[][] trainData, out double[][] testData)
        {
            Random rnd = new Random(seed);
            int totRows = allData.Length;
            int numTrainRows = (int)(totRows * trainPct); // usually 0.80
            int numTestRows = totRows - numTrainRows;
            trainData = new double[numTrainRows][];
            testData = new double[numTestRows][];

            double[][] copy = new double[allData.Length][]; // ref copy of data
            for (int i = 0; i < copy.Length; ++i)
                copy[i] = allData[i];

            for (int i = 0; i < copy.Length; ++i) // scramble order
            {
                int r = rnd.Next(i, copy.Length); // use Fisher-Yates
                double[] tmp = copy[r];
                copy[r] = copy[i];
                copy[i] = tmp;
            }
            for (int i = 0; i < numTrainRows; ++i)
                trainData[i] = copy[i];

            for (int i = 0; i < numTestRows; ++i)
                testData[i] = copy[i + numTrainRows];
        } // 

        private void CreateData(double[][] allData, out double[][] outData, int seed)
        {
            Random rnd = new Random(seed);
            outData = new double[allData.Length][];
            outData = allData;
        }
        #endregion
    }
}
