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
using Common.Savers;

namespace MarkovModule.Models
{
    public class DiscreteModel
    {
        private int States { get; set; }
        private int[] LearnedPrediction { get; set; }
        private HiddenMarkovModel Model { get; set; }
        private RunningMarkovStatistics MarkovStatistics { get; set; }
        private HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> Classifier { get; set; }
        private Queue<int> StatesQueue { get; set; }

        private double[] ProbabilityA { get; set; }

        private static readonly Logger Logger = LogManager.GetLogger("Discrete model");

        public DiscreteModel(int states, int[] learnedPrediction)
        {
            States = states;
            LearnedPrediction = learnedPrediction;
            StatesQueue = new Queue<int>(5);
            SetUpModel();
            ProbabilityA = CreateInitial();
        }

        public DiscreteModel(HiddenMarkovModel model, HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> classifier)
        {
            Model = model;
            States = classifier.NumberOfClasses;
            MarkovStatistics = new RunningMarkovStatistics(Model);
            Classifier = classifier;
            Classifier.Sensitivity = 1e-150;
            StatesQueue = new Queue<int>(5);
            Model.Algorithm = HiddenMarkovModelAlgorithm.Viterbi;
            ProbabilityA = CreateInitial();
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
            var decision = Classifier.Decide(sequence);

            var logLikelihoods = new List<double>();

            foreach (var model in Classifier.Models)
            {
                logLikelihoods.Add(model.LogLikelihood(sequence));
            }
            var classifierProbability = Classifier.Probability(sequence);

            var operation = logLikelihoods.ToList().IndexOf(logLikelihoods.Max());

            if (decision  == -1)
            {
                Logger.Warn("The operation is: {}, but it is out the refrence. \n It is possible, that the robot is demage, please call the maintenance", operation);
            }
            ComputeCurrentState(logLikelihoods.ToArray());
            return new Decision(classifierProbability, Classifier.Threshold.LogLikelihood(sequence) , operation + 1);
        }

        public void Decide(double[][] sequence, string filePath)
        {
            var decisionList = new List<double[]>();
            var samples = new List<double[]>();
            var logLikelihoods = new List<double>();
            foreach (var line in sequence)
            {
                samples.Add(line);
                foreach (var model in Classifier.Models)
                {
                    logLikelihoods.Add(model.LogLikelihood(samples.ToArray()));
                }                
                decisionList.Add(logLikelihoods.ToArray());
                logLikelihoods.Clear();
            }
            ComputeCurrentState(logLikelihoods.ToArray());
            CsvSavers.SaveLogLikelihoodEvaluation(filePath, decisionList.ToArray());
        }

        private double[,] CreateTransitionMatrix()
        {
            var transition = CalculateFrequency().ToJagged();
            return NormalizeTransition(transition).ToArray().ToMatrix();

        }

        private void ComputeCurrentState(double[] probabilityC)
        {
            var probabilityD = Model.LogTransitions.Dot(ProbabilityA);
            var newProbabilityA = probabilityD.Dot(Matrix.Diagonal(probabilityC));

            ProbabilityA = newProbabilityA;

            var retVal = ProbabilityA.IndexOf(ProbabilityA.Min());
        }

        private double[,] CalculateFrequency()
        {
            var transition = Matrix.Create(States, States, 0.5);

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
