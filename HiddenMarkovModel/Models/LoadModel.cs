using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Accord.IO;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;

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
            return Serializer.Load<HiddenMarkovClassifier<MultivariateNormalDistribution, double[]>>(Path.Combine(FilePath, "markov_model.bin"));
        }
    }
}
