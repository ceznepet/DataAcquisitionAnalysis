using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using Accord.Math.Random;
using Accord.Statistics.Analysis;
using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;
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
        private static readonly Logger Logger = LogManager.GetLogger("Learning");

        public Learning(Dictionary<int, List<double[]>> trainData, Dictionary<int, List<double[]>> testData)
        {
            TrainData = trainData;
            TestData = testData;
        }

        public static void StartTeaching(Dictionary<int, List<double[]>> trainData, Dictionary<int, List<double[]>> testData, int dimension)
        {
            new Learning(trainData, testData).TeachModel(dimension);
        }

        public void TeachModel(int dimension)
        {
            Generator.Seed = 0;

            var length = TrainData.Count();
            var states = dimension;
            var sequences = new double[length + 1][][];
            var labels = new int[length + 1];
            for (var i = 1; i <= length; i++)
            {
                sequences[i - 1] = TrainData[i].ToArray();
                labels[i - 1] = i;
            }

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
                    Topology = new Ergodic(states),

                    Emissions = (j) => new MultivariateNormalDistribution(mean: priorM.Generate(), covariance: priorC.Generate()),

                    Tolerance = 1e-6,
                    MaxIterations = 0,

                    FittingOptions = new NormalOptions()
                    {
                        Diagonal = true,
                        //Robust = true,
                        Regularization = 1e-6
                    }
                }
            };

            Learner.ParallelOptions.MaxDegreeOfParallelism = 2;

            Classifier = Learner.Learn(sequences, labels);
            Logger.Debug("End of Learning phase...");
            var trainPredicted = Classifier.Decide(sequences);

            var m1 = new GeneralConfusionMatrix(predicted: trainPredicted, expected: labels);
            var trainAcc = m1.Accuracy;

            Logger.Info("Check of performance: {0}", trainAcc);

            var testData = new double[length][][];
            var testOutputs = new int[length];
            for (var i = 1; i <= length; i++)
            {
                testData[i - 1] = TestData[i].ToArray();
                testOutputs[i - 1] = i;
            }

            testData = testData.Apply(Accord.Statistics.Tools.ZScores);

            var testPredict = Classifier.Decide(testData);

            var m2 = new GeneralConfusionMatrix(testPredict, testOutputs);
            var trainAccTest = m2.Accuracy;
            Logger.Info("Check of performance: {0}", trainAccTest);

        }

        private double[][] ReduceDimension(int dimension, double[][] data)
        {

            var pca = new PrincipalComponentAnalysis()
            {
                Method = PrincipalComponentMethod.Standardize,
                Whiten = false
            };

            var transform = pca.Learn(data);

            pca.NumberOfOutputs = dimension;

            return pca.Transform(data);
        }
    }
}
