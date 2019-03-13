using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov.Learning;

namespace HiddenMarkovModel.Models
{
    public class Learning
    {
        private HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]> Teacher {get; set; }
        private MultivariateNormalDistribution InitialDistribution { get; set; }
        private string Folder { get; set; }

        public Learning(string folder)
        {
            Folder = folder;
        }
    }
}
