using Accord.IO;
using Accord.Math;
using Accord.Statistics.Models.Markov;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accord.MachineLearning;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Models.Markov.Learning;
using NLog;

namespace HMModel.Models
{
    public class DiscreteModel
    {
        private int States { get; set; }
        private int[] LearnedPrediction { get; set; }
        private HiddenMarkovModel<GeneralDiscreteDistribution, int> Model { get; set; }
        private HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> Classifier { get; set; }
        private Queue<int> StatesQueue { get; set; }
        private static readonly Logger Logger = LogManager.GetLogger("Discrete model");

        public DiscreteModel(int states, int[] learnedPrediction)
        {
            States = states;
            LearnedPrediction = learnedPrediction;
            StatesQueue = new Queue<int>(5);
            SetUpModel();
        }

        public DiscreteModel(HiddenMarkovModel<GeneralDiscreteDistribution, int> model, HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> classifier)
        {
            Model = model;
            Classifier = classifier;
            StatesQueue = new Queue<int>(5);
            Model.Algorithm = HiddenMarkovModelAlgorithm.Viterbi;
        }

        private void SetUpModel()
        {
            var transition = CreateTransitionMatrix();
            
            var emission = Matrix.Diagonal(States, States , 1.0);
            var initial = CreateInitial();
            var emissions = GeneralDiscreteDistribution.FromMatrix(emission);

            Model =new HiddenMarkovModel<GeneralDiscreteDistribution, int>(transition, emissions, initial);

            var path = Path.Combine(@"../../../../Models", "dis_markov_model.bin");
            Logger.Info("Model saved.");
            Serializer.Save(Model, path);

        }

        private double[] CreateInitial()
        {
            var initial = Vector.Create(States, 1.0 / States);
            return initial;
        }

        public Decision Decide(double[][] sequence)
        {
            sequence = Accord.Statistics.Tools.ZScores(sequence);
            var decision = Classifier.Decide(sequence);
            //var p = Classifier.ToMultilabel().Probabilities(sequence);
            var classifierProbability = Classifier.LogLikelihood(sequence);
            StatesQueue.Enqueue(decision == 22 ? 0 : decision);
            var probability = Model.Predict(StatesQueue.ToArray().Take(StatesQueue.Count).ToArray());
            CleanStatesQueue();
            return new Decision(classifierProbability, probability, decision);
        }

        private void CleanStatesQueue()
        {
            if (StatesQueue.Count == 5)
            {
                StatesQueue.Dequeue();
            }
        }

        private double[,] CreateTransitionMatrix()
        {
            var transition = CalculateFrequency().ToJagged();
            //transition[0] = Vector.Create(States + 1, 1.0);
            return NormalizeTransition(transition).ToArray().ToMatrix();

        }

        private double[,] CalculateFrequency()
        {
            var transition = Matrix.Create(States, States, 0.0);

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
