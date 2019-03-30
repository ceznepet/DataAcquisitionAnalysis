using Accord.IO;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using System.IO;
using Accord.Statistics.Distributions.Univariate;

namespace HMModel.Models
{
    public class LoadModel
    {
        private string FilePath { get; set; }


        public LoadModel(string filePath)
        {
            FilePath = filePath;
        }


        public static HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> LoadMarkovClassifier(string filePath)
        {
            return new LoadModel(filePath).LoadClassifier();
        }

        private HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> LoadClassifier()
        {
            return Serializer.Load<HiddenMarkovClassifier<MultivariateNormalDistribution, double[]>>(Path.Combine(FilePath, "markov_model_20.bin"));
        }

        public static HiddenMarkovModel<GeneralDiscreteDistribution, int> LoadMarkovModel(string filePath)
        {
            return new LoadModel(filePath).LoadMarkov();
        }

        private HiddenMarkovModel<GeneralDiscreteDistribution, int> LoadMarkov()
        {
            return Serializer.Load<HiddenMarkovModel<GeneralDiscreteDistribution, int>>(Path.Combine(FilePath, "dis_markov_model.bin"));
        }

    }
}
