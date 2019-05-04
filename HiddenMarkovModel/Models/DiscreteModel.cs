using System;
using Accord.IO;
using Accord.Math;
using Accord.Statistics.Models.Markov;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Running;
using NLog;
using Common.Savers;

namespace MarkovModule.Models
{
    public class DiscreteModel
    {
        private int States { get; set; }
        private RunningMarkovStatistics MarkovStatistics { get; set; }
        private HiddenMarkovModel Model { get; set; }
        private HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> Classifier { get; set; }

        private double[] ProbabilityA { get; set; }

        private static readonly Logger Logger = LogManager.GetLogger("Discrete model");

        public DiscreteModel(int states)
        {
            States = states;
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

            var logLikelihood = logLikelihoods.ToArray();
            var operation = logLikelihood.IndexOf(logLikelihoods.Max());

            if (decision  == -1)
            {
                Logger.Warn("The operation is: {}, but it is out the refrence. \n It is possible, that the robot is demage, please call the maintenance", operation);
            }
            return new Decision(classifierProbability, Classifier.Threshold.LogLikelihood(sequence) , ComputeCurrentState(logLikelihood) + 1);
        }

        public void OnlineDecide(double[][] sequence)
        {
            var decision = Classifier.Decide(sequence);
            var logLikelihoods = new List<double>();

            foreach (var model in Classifier.Models)
            {
                logLikelihoods.Add(model.LogLikelihood(sequence));
            }
            var operation = logLikelihoods.IndexOf(logLikelihoods.Max()) + 1;
            if (decision == -1)
            {
                Logger.Warn("The operation is: {}, but it is out the refrence. \n It is possible, that the robot is demage, please call the maintenance", operation);
            }
            else
            {
                Logger.Info("Current operation is: {}", decision);
            }
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
            CsvSavers.SaveLogLikelihoodEvaluation(filePath, decisionList.ToArray());
        }

        private double[,] CreateTransitionMatrix()
        {
            var transition = CalculateFrequency().ToJagged();
            return NormalizeTransition(transition).ToArray().ToMatrix();

        }

        private int ComputeCurrentState(double[] probabilityC)
        {
            probabilityC = probabilityC.Select(item => item / probabilityC.Sum()).ToArray();
            var probabilityD = Model.LogTransitions.Dot(ProbabilityA);
            probabilityD = probabilityD.Select(item => item / probabilityD.Sum()).ToArray();
            var newProbabilityA = Matrix.Diagonal(probabilityD).Dot(probabilityC);

            ProbabilityA = newProbabilityA;
            ProbabilityA = ProbabilityA.Select(item => item / ProbabilityA.Sum()).ToArray();
            return ProbabilityA.IndexOf(ProbabilityA.Min());
       }

        private double[,] CalculateFrequency()
        {
            var transition = Matrix.Create(States, States, 0.001);

            var prevState = 21;
            int k = 0;
            for (var i = 0; i < 22; i++)
            {
                var state = k;
                transition[prevState, state] += 100;
                prevState = state;
                k++;
            }

            return transition;
        }

        private static IEnumerable<double[]> NormalizeTransition(IEnumerable<double[]> transition)
        {
            return transition.Select(row => row.Divide(row.Sum()));
        }


    }
}
