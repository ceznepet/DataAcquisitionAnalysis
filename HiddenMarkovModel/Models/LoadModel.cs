﻿using Accord.IO;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using System.IO;
using Accord.Statistics.Distributions.Univariate;

namespace MarkovModule.Models
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
            return Serializer.Load<HiddenMarkovClassifier<MultivariateNormalDistribution, double[]>>(Path.Combine(FilePath, "markov_model_14_all.bin"));
        }

        public static HiddenMarkovModel LoadMarkovModel(string filePath)
        {
            return new LoadModel(filePath).LoadMarkov();
        }

        private HiddenMarkovModel LoadMarkov()
        {
            return Serializer.Load<HiddenMarkovModel>(Path.Combine(FilePath, "dis_markov_model.bin"));
        }

    }
}
