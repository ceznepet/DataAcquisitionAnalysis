using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;

namespace HiddenMarkovModel.Models
{
    public class Learning
    {
        private HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]> Teacher {get; set; }
        private HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> Classifier { get; set; }
        private MultivariateNormalDistribution InitialDistribution { get; set; }
        private IOrderedEnumerable<KeyValuePair<int, List<double[]>>> OrderedOperations { get; set; }

        public Learning(IOrderedEnumerable<KeyValuePair<int, List<double[]>>> orderedOperations)
        {
            OrderedOperations = orderedOperations;
        }

        public void TeachModel()
        {
            var sequences = new double[OrderedOperations.Count()][][];
            var length = OrderedOperations.Count();
            var labels = new int[length];
            for (var i = 0; i < length; i++)
            {
                sequences[i] = OrderedOperations.ElementAt(i).Value.Take(400).ToArray();
                labels[i] = i;
            }

            var initialDensity = new MultivariateNormalDistribution(30);

            Classifier = new HiddenMarkovClassifier<MultivariateNormalDistribution, double[]>(
                classes: length, topology: new Forward(2), initial: initialDensity);

            Teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]>(Classifier)
            {
                // Train each model until the log-likelihood changes less than 0.0001
                Learner = modelIndex => new BaumWelchLearning<MultivariateNormalDistribution, double[], NormalOptions>(Classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0,

                    FittingOptions = new NormalOptions()
                    {
                        Diagonal = true,      // only diagonal covariance matrices
                        Regularization = 1e-5 // avoid non-positive definite errors
                    }
                }
            };

            Teacher.Learn(sequences, labels);

            double likelihood, likelihood2;

            var c1 = Classifier.Decide(sequences[0]);
            likelihood = Classifier.Probability(sequences[0]);
            

            // Try to classify the second sequence (output should be 1)
            var c2 = Classifier.Decide(OrderedOperations.ElementAt(1).Value.Skip(300).Take(300).ToArray());
            likelihood2 = Classifier.Probability(OrderedOperations.ElementAt(1).Value.Skip(300).Take(300).ToArray());

            Console.WriteLine("C1: {0} and is {1}, C2: {2} and is {3}", likelihood, c1, likelihood2, c2);
        }
    }
}
