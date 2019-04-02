using System;
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
using Accord.Statistics.Running;
using NLog;

namespace HMModel.Models
{
    public class DiscreteModel
    {
        private int States { get; set; }
        private int[] LearnedPrediction { get; set; }
        private HiddenMarkovModel Model { get; set; }
        private RunningMarkovStatistics MarkovStatistics { get; set; }
        private HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> Classifier { get; set; }
        private Queue<int> StatesQueue { get; set; }
        private static readonly Logger Logger = LogManager.GetLogger("Discrete model");

        public DiscreteModel(int states, int[] learnedPrediction)
        {
            States = states + 1;
            LearnedPrediction = learnedPrediction;
            StatesQueue = new Queue<int>(5);
            SetUpModel();
        }

        public DiscreteModel(HiddenMarkovModel model, HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> classifier)
        {
            Model = model;
            MarkovStatistics = new RunningMarkovStatistics(Model);
            Classifier = classifier;
            StatesQueue = new Queue<int>(5);
            Model.Algorithm = HiddenMarkovModelAlgorithm.Viterbi;
        }

        private void SetUpModel()
        {
            var transition = CreateTransitionMatrix();
            
            var emission = Matrix.Diagonal(States, States, 1.0);
            var initial = CreateInitial();

            Model = new HiddenMarkovModel(transition, emission, initial);

            var path = Path.Combine(@"../../../../Models", "dis_markov_model.bin");
            Logger.Info("Model saved.");
            Serializer.Save(Model, path);

        }

        private double[] CreateInitial()
        {
            var initial = Vector.Create(States, 1.0 / (States));
            return initial;
        }

        public Decision Decide(double[][] sequence)
        {
            sequence = Accord.Statistics.Tools.ZScores(sequence);
            var decision = Classifier.Decide(sequence);
           

            var classifierProbability = Classifier.Probability(sequence);

            var probability = MarkovStatistics.Peek(decision == 22 ? 0 : decision);

            if (probability < -5)
            {
                Logger.Info("Predicted: {}", decision);
                Logger.Info("Should be: {}", MarkovStatistics.CurrentState);
            }
            else
            {
                MarkovStatistics.Push(decision == 22 ? 0 : decision);
                StatesQueue.Enqueue(decision);
            }
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
            transition[0] = Vector.Create(States, 1.0);
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
                    prevState = LearnedPrediction[i];
                    continue;
                }

                var state = LearnedPrediction[i];
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
