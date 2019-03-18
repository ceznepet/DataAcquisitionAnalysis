using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using Accord.Math;
using System.Linq;
using Accord.Statistics.Distributions.Univariate;
using System.Collections.Generic;
using NLog;

namespace HMModel.Models
{
    public class Predictor
    {
        private HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> Continues { get; set; }
        private HiddenMarkovModel<GeneralDiscreteDistribution, double> Discrete { get; set; }
        private static readonly Logger Logger = LogManager.GetLogger("Classification");

        public Predictor(HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> continues)
        {
            Continues = continues;
            CreatDiscrete(Continues.NumberOfClasses);
        }

        private void CreatDiscrete(int operation)
        {
            var init_proba = Enumerable.Repeat(1.0 / operation, operation).ToArray();

            var transition = GetInitTransition(operation);

            var emissions = GetEmissions(operation).ToArray();

            Discrete = new HiddenMarkovModel<GeneralDiscreteDistribution, double>(transition.ToJagged(), emissions, init_proba);
        }

        private double[,] GetInitTransition(int dimension)
        {
            var transition = new double[dimension][];

            for(var i = 0; i < dimension; i++)
            {
                transition[i] = Enumerable.Repeat(1.0/dimension, dimension).ToArray();
            }

            return transition.ToMatrix();
        }

        private IEnumerable<GeneralDiscreteDistribution> GetEmissions(int operation)
        {
            for(var i = 0; i < operation; i++)
            {
                yield return new GeneralDiscreteDistribution(Enumerable.Repeat(1.0 / operation, operation).ToArray());
            }
        }

        public void PredictState(double[] emission)
        {
            var predict = Continues.Probabilities(emission.ToJagged());

            var state = Discrete.Decide(predict);
            Logger.Info("Operation is: {}.", state);
        }
    }
}
