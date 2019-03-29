using Accord.IO;
using Accord.Math;
using Accord.Statistics.Models.Markov;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Models.Markov.Topology;
using NLog;

namespace HMModel.Models
{
    public class DiscreteModel
    {
        private int States { get; set; }
        private int[] LearnedPrediction { get; set; }
        private HiddenMarkovModel<GeneralDiscreteDistribution, int> Model { get; set; }
        private HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> Classifier { get; set; }
        private static readonly Logger Logger = LogManager.GetLogger("Discrete model");

        public DiscreteModel(int states, int[] learnedPrediction)
        {
            States = states;
            LearnedPrediction = learnedPrediction;
            SetUpModel();
        }

        public DiscreteModel(HiddenMarkovModel<GeneralDiscreteDistribution, int> model, HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> classifier)
        {
            Model = model;
            Classifier = classifier;
            Model.Algorithm = HiddenMarkovModelAlgorithm.Viterbi;
        }

        private void SetUpModel()
        {
            var transition = CreateTransitionMatrix();
            
            var emission = Matrix.Diagonal(States, States, 1.0);
            var initial = Vector.Create(States, 1.0 / States);

            Model = HiddenMarkovModel.CreateDiscrete(transition, emission, initial);

            var path = Path.Combine(@"../../../../Models", "dis_markov_model.bin");
            Logger.Info("Model saved.");
            Serializer.Save(Model, path);

        }

        public Decision Decide(double[][] sequence)
        {
            sequence = Accord.Statistics.Tools.ZScores(sequence);
            var decision = Classifier.Decide(sequence);
            var classifierProbability = Classifier.Probability(sequence);
            var toDecide = new[] {decision};
            var state = Model.Decide(toDecide);
            var probability = Model.Probability(state);

            return new Decision(classifierProbability, probability, state[0]);
        }

        private double[,] CreateTransitionMatrix()
        {
            var transition = CalculateFrequency();

            return NormalizeTransition(transition.ToJagged()).ToArray().ToMatrix();

        }

        private double[,] CalculateFrequency()
        {
            var transition = new double[States, States];

            var prevState = 0;

            for (var i = 0; i < LearnedPrediction.Length; i++)
            {
                if (i == 0)
                {
                    prevState = LearnedPrediction[i] - 1;
                    continue;
                }

                var state = LearnedPrediction[i] - 1;
                transition[prevState, state] += 1;
                prevState = state;
            }

            return transition;
        }

        private static IEnumerable<double[]> NormalizeTransition(IEnumerable<double[]> transition)
        {
            return transition.Select(row => row.Divide(row.Sum()));
        }


    }
}
