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
        private HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]> Teacher { get; set; }
        private HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> Classifier { get; set; }
        private MultivariateNormalDistribution InitialDistribution { get; set; }
        private IOrderedEnumerable<KeyValuePair<int, List<double[]>>> OrderedOperations { get; set; }
        private static readonly Logger Logger = LogManager.GetLogger("Teaching");

        public Learning(IOrderedEnumerable<KeyValuePair<int, List<double[]>>> orderedOperations)
        {
            OrderedOperations = orderedOperations;
        }

        public void TeachModel(int dimension)
        {
            Generator.Seed = 0;
            //malo dat...
            var length = 10; //OrderedOperations.Count();
            var sequences = new double[length][][];
            var labels = new int[length];
            for (var i = 0; i < length; i++)
            {
                sequences[i] = OrderedOperations.ElementAt(i).Value.ToArray();
                if (i % 2 == 0)
                {
                    labels[i] = 0;
                }
                else
                {
                    labels[i] = 1;
                }
                //labels[i] = i;
            }

            //sequences = sequences.Apply(Accord.Statistics.Tools.ZScores);

            var priorC = new WishartDistribution(dimension: dimension, degreesOfFreedom: dimension + 5);
            var priorM = new MultivariateNormalDistribution(dimension: dimension);

            Teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]>()
            {
                Learner = (i) => new BaumWelchLearning<MultivariateNormalDistribution, double[], NormalOptions>()
                {
                    Topology = new Ergodic(2),

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
            //Teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
            Classifier = Teacher.Learn(sequences, labels);

            Learner.ParallelOptions.MaxDegreeOfParallelism = 5;

            Classifier = Learner.Learn(sequences, labels);
            Logger.Debug("End of Learning phase...");
            var trainPredicted = Classifier.Decide(sequences);

            var m1 = new GeneralConfusionMatrix(predicted: trainPredicted, expected: labels);
            var trainAcc = m1.Accuracy;

            Console.WriteLine("Check of performance: {0}", trainAcc);

            var testData = new double[length][][];
            var testOutputs = new int[length];
            var k = 0;
            for (var i = 10; i < 10 + length; i++)
            {
                testData[k] = OrderedOperations.ElementAt(i).Value.ToArray();
                Logger.Debug(i);
                if (i % 2 == 0)
                {
                    testOutputs[k] = 0;
                }
                else
                {
                    testOutputs[k] = 1;
                }

                k++;
                //testOutputs[i] = i;
            }

            //testData = testData.Apply(Accord.Statistics.Tools.ZScores);

            var testPredict = Classifier.Decide(testData);

            var m2 = new GeneralConfusionMatrix(testPredict, testOutputs);
            var trainAccTest = m2.Accuracy;
            Console.WriteLine("Check of performance: {0}", trainAccTest);

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
