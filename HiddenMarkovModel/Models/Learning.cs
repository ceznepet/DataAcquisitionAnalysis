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
        private List<Operation> Data { get; }
        private static readonly Logger Logger = LogManager.GetLogger("Learning");

        public Learning(Dictionary<int, List<double[]>> trainData, Dictionary<int, List<double[]>> testData)
        {
            TrainData = trainData;
            TestData = testData;
        }

        public Learning(IEnumerable<Operation> trainData, int skip, int take)
        {
            var operations = trainData.ToList();
            foreach (var data in operations)
            {
                data.Data = data.Data.Select(element => element.Skip(skip).Take(take).ToArray()).ToArray();
            }

            Data = operations;

        }

        public static void StartTeaching(Dictionary<int, List<double[]>> trainData, Dictionary<int, List<double[]>> testData, int dimension)
        {
            new Learning(trainData, testData).TeachModel(dimension, false);
        }

        public static void StartTeaching(IEnumerable<Operation> trainData, int skip, int take)
        {
            new Learning(trainData, skip, take).TeachModel(take, true);
        }

        public void TeachModel(int dimension, bool operation)
        {
            Generator.Seed = 0;

            var length = TrainData.Count();
            var states = dimension;
            var sequences = ToSequence(operation);
            var labels = GetLabels(operation);

            labels[length] = 0;
            sequences[length] = new double[][]
            {
                Enumerable.Repeat(0.0, dimension).ToArray()
            };
            sequences = sequences.Apply(Accord.Statistics.Tools.ZScores);

            var priorC = new WishartDistribution(dimension: dimension, degreesOfFreedom: dimension + 5);
            var priorM = new MultivariateNormalDistribution(dimension: dimension);
            Logger.Info("Preparation of model...");

            var crossvalidation =
                new CrossValidation<HiddenMarkovClassifier<MultivariateNormalDistribution, double[]>, double[][]>
                {
                    K = 3, // Use 3 folds in cross-validation
                    Learner = (s) => new HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]>()
                    {
                        Learner = (p) =>
                            new BaumWelchLearning<MultivariateNormalDistribution, double[], NormalOptions>()
                            {
                                Topology = new Ergodic(states),
                                Emissions = (j) =>
                                    new MultivariateNormalDistribution(mean: priorM.Generate(),
                                        covariance: priorC.Generate()),
                                Tolerance = 1e-6,
                                MaxIterations = 0,
                                FittingOptions = new NormalOptions()
                                {
                                    Diagonal = true,
                                    //Robust = true,
                                    Regularization = 1e-6
                                }
                            }
                    },
                    Loss = (expected, actual, p) =>
                    {
                        var cm = new GeneralConfusionMatrix(classes: p.Model.NumberOfClasses, expected: expected,
                            predicted: actual);
                        p.Variance = cm.Variance;
                        return p.Value = cm.Kappa;
                    },
                    Stratify = false,
                    ParallelOptions = {MaxDegreeOfParallelism = 1},
                };

            var result = crossvalidation.Learn(sequences, labels);

            Logger.Info("Cross-Validation done...");
            // If desired, compute an aggregate confusion matrix for the validation sets:
            var gcm = result.ToConfusionMatrix(sequences, labels);

            // Finally, access the measured performance.
            var trainingErrors = result.Training.Mean;
            var validationErrors = result.Validation.Mean;

            var trainingErrorVar = result.Training.Variance;
            var validationErrorVar = result.Validation.Variance;

            var trainingErrorPooledVar = result.Training.PooledVariance;
            var validationErrorPooledVar = result.Validation.PooledVariance;

            var accuracy = gcm.Accuracy;
            Logger.Info("Training Error: {}", trainingErrors);
            Logger.Info("Validation Error: {}", validationErrors);
            Logger.Info("Training error variance: {}", trainingErrorVar);
            Logger.Info("Validation error variance: {}", validationErrorVar);
            Logger.Info("Training error pooled variance: {}", trainingErrorPooledVar);
            Logger.Info("Validation error pooled variance: {}", validationErrorPooledVar);
            Logger.Info("General confusion matrix accuracy: {}", accuracy);
        }

        private double[][][] ToSequence(bool operation)
        {
            if (operation)
            {
                return Data.Select(element => element.Data).ToArray();
            }
            var length = TrainData.Count();
            var sequences = new double[length + 1][][];
            for (var i = 1; i <= length; i++)
            {
                sequences[i - 1] = TrainData[i].ToArray();
            }

            return sequences;
        }

        private int[] GetLabels(bool operation)
        {
            if (operation)
            {
                return Data.Select(element => int.Parse(element.Name)).ToArray();
            }

            var length = TrainData.Count();
            var labels = new int[length + 1];
            for (var i = 1; i <= length; i++)
            {
                labels[i - 1] = i;
            }

            return labels;
        }
    }
}
