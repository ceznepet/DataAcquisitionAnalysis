using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using Accord.Math.Random;
using Accord.Statistics.Analysis;
using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Kernels;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;
using Common.Logging;
using NLog;

namespace HiddenMarkovModel.Models
{
    public class Learning
    {
        private HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]> Learner { get; set; }
        private HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> Classifier { get; set; }
        private MultivariateNormalDistribution InitialDistribution { get; set; }
        private IOrderedEnumerable<KeyValuePair<int, List<double[]>>> OrderedOperations { get; set; }
        private static readonly Logger Logger = LogManager.GetLogger("Learning");

        public Learning(IOrderedEnumerable<KeyValuePair<int, List<double[]>>> orderedOperations)
        {
            OrderedOperations = orderedOperations;
        }

        public static void StartTeaching(IOrderedEnumerable<KeyValuePair<int, List<double[]>>> orderedOperations, int dimension)
        {
            new Learning(orderedOperations).TeachModel(dimension);
        }

        public void TeachModel(int dimension)
        {
            Generator.Seed = 0;

            var length = OrderedOperations.Count();
            var states = dimension;
            var sequences = new double[length][][];
            var labels = new int[length];
            for (var i = 0; i < length; i++)
            {
                sequences[i] = OrderedOperations.ElementAt(i).Value.Take(5000).ToArray();
                labels[i] = i;
            }

            //sequences = sequences.Apply(Accord.Statistics.Tools.ZScores);

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
                       // Robust = true,
                        Regularization = 1e-6
                    }
                }
            };

            Learner.ParallelOptions.MaxDegreeOfParallelism = 10;

            Classifier = Learner.Learn(sequences, labels);
            Logger.Debug("End of Learning phase...");
            var trainPredicted = Classifier.Decide(sequences);

            var m1 = new GeneralConfusionMatrix(predicted: trainPredicted, expected: labels);
            var trainAcc = m1.Accuracy;

            Logger.Info("Check of performance: {0}", trainAcc);

            var testData = new double[length][][];
            var testOutputs = new int[length];
            var k = 0;
            for (var i = 0; i < length; i++)
            {
                testData[i] = OrderedOperations.ElementAt(i).Value.Skip(5000).Take(100).ToArray();
                testOutputs[i] = i;
            }

            //testData = testData.Apply(Accord.Statistics.Tools.ZScores);

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
