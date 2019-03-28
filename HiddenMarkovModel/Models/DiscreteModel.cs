using Accord.IO;
using Accord.Math;
using Accord.Statistics.Models.Markov;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HMModel.Models
{
    public class DiscreteModel
    {
        private int States { get; set; }
        private int[] LearnedPrediction { get; set; }
        private HiddenMarkovModel Model { get; set; }

        public DiscreteModel(int states, int[] learnedPrediction)
        {
            States = states;
            LearnedPrediction = learnedPrediction;
            SetUpModel();
        }

        public DiscreteModel(HiddenMarkovModel model)
        {
            Model = model;
        }

        private void SetUpModel()
        {
            var transition = CreateTransitionMatrix();
            var emission = Matrix.Diagonal(States, States, 1.0);
            var initial = Vector.Create(States, 0.0);

            Model = new HiddenMarkovModel(transition, emission, initial);

            var path = Path.Combine(@"../../../../Models", "dis_markov_model.bin");
            Serializer.Save(Model, path);

        }

        public IEnumerable<Decision> Decide(int[] sequence)
        {
            foreach (var decision in sequence)
            {
                var variable = new[] { decision };

                yield return new Decision(Model.LogLikelihoods(variable), Model.Decide(variable)[0]);
            }
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

        private IEnumerable<double[]> NormalizeTransition(IEnumerable<double[]> transition)
        {
            return transition.Select(row => row.Divide(row.Sum()));
        }


    }
}
