using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning;
using Accord.MachineLearning.Performance;
using Accord.Math;
using Accord.Math.Optimization.Losses;
using Accord.Math.Random;
using Accord.Statistics.Analysis;
using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Kernels;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;
using Common.Models;
using MongoDB.Bson;
using NLog;

namespace HMModel.Models
{
    public class Learning
    {
        private HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]> Learner { get; set; }
        private HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> Classifier { get; set; }
        private MultivariateNormalDistribution InitialDistribution { get; set; }
        private Dictionary<int, List<double[]>> TrainData { get; set; }
        private Dictionary<int, List<double[]>> TestData { get; set; }
        private List<Operation> DataToTrain { get; }
        private List<Operation> DataToTest { get; }
        private static readonly Logger Logger = LogManager.GetLogger("Learning");

        public Learning(Dictionary<int, List<double[]>> trainData, Dictionary<int, List<double[]>> testData)
        {
            TrainData = trainData;
            TestData = testData;
        }

        public Learning(IEnumerable<Operation> trainData, IEnumerable<Operation> testData, int skip, int take)
        {
            var operations = trainData.ToList();
            foreach (var data in operations)
            {
                data.Data = data.Data.Select(element => element.Skip(skip).Take(take).ToArray()).ToArray();
            }

            DataToTrain = operations;

            operations = testData.ToList();
            foreach (var data in operations)
            {
                data.Data = data.Data.Select(element => element.Skip(skip).Take(take).ToArray()).ToArray();
            }

            DataToTest = operations;

        }

        public static void StartTeaching(Dictionary<int, List<double[]>> trainData, Dictionary<int, List<double[]>> testData, int dimension)
        {
            new Learning(trainData, testData).TeachModel(dimension, false);
        }

        public static void StartTeaching(IEnumerable<Operation> trainData, IEnumerable<Operation> testData, int skip, int take)
        {
            new Learning(trainData, testData, skip, take).TeachModel(take, true);
        }

        public void TeachModel(int dimension, bool operation)
        {
            Generator.Seed = 0;

            var length = 22;
            var states = dimension;
            var sequences = ToSequence(operation, true);
            var labels = GetLabels(operation, true);

            labels[length] = 0;
            sequences[length] = new double[][]
            {
                Enumerable.Repeat(0.0, dimension).ToArray()
            };
            sequences = sequences.Apply(Accord.Statistics.Tools.ZScores);

            var priorC = new WishartDistribution(dimension: dimension, degreesOfFreedom: dimension + 5);
            var priorM = new MultivariateNormalDistribution(dimension: dimension);
            Logger.Info("Preparation of model...");

            Learner = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]>()
            {
                Learner = (i) => new BaumWelchLearning<MultivariateNormalDistribution, double[], NormalOptions>()
                {
                    Topology = new Ergodic(6),

                    Emissions = (j) => new MultivariateNormalDistribution(mean: priorM.Generate(), covariance: priorC.Generate()),

                    Tolerance = 1e-6,
                    MaxIterations = 0,

                    FittingOptions = new NormalOptions()
                    {
                        Diagonal = true,
                        // Robust = true,
                        Regularization = 1e-6
                    }
                }
            };

            //Learner.ParallelOptions.MaxDegreeOfParallelism = 5;

            Classifier = Learner.Learn(sequences, labels);
            Logger.Debug("End of Learning phase...");
            var trainPredicted = Classifier.Decide(sequences);

            var m1 = new GeneralConfusionMatrix(predicted: trainPredicted, expected: labels);
            var trainAcc = m1.Accuracy;

            Logger.Info("Check of performance: {0}", trainAcc);

            var testData = ToSequence(operation, false);
            var testOutputs = GetLabels(operation, false);

            testData = testData.Apply(Accord.Statistics.Tools.ZScores);

            var testPredict = Classifier.Decide(testData);

            var m2 = new GeneralConfusionMatrix(testPredict, testOutputs);
            var trainAccTest = m2.Accuracy;
            Logger.Info("Check of performance: {0}", trainAccTest);

        }

        private double[][][] ToSequence(bool operation, bool train)
        {
            if (operation)
            {
                return train ? DataToTrain.Select(element => element.Data).ToArray() 
                             : DataToTest.Select(element => element.Data).ToArray();
            }
            var length = 22;
            var sequences = new double[length + 1][][];
            for (var i = 1; i <= length; i++)
            {
                sequences[i - 1] = train ? TrainData[i].ToArray() 
                                         : TestData[i].ToArray();
            }

            return sequences;
        }

        private int[] GetLabels(bool operation, bool train)
        {
            if (operation)
            {
                return train ? DataToTrain.Select(element => int.Parse(element.Name)).ToArray() 
                             : DataToTest.Select(element => int.Parse(element.Name)).ToArray();
            }

            var length = 22;
            var labels = new int[length + 1];
            for (var i = 1; i <= length; i++)
            {
                labels[i - 1] = i;
            }

            return labels;
        }
    }
}
