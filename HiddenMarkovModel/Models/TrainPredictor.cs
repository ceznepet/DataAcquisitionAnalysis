using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Accord.Math;
using NLog;


namespace HMModel.Models
{
    public class TrainPredictor
    {
        public static readonly Logger Logger = LogManager.GetLogger("Training of Predictor");

        public TrainPredictor(int[] prediction, int states)
        {
            OperationPrediction = prediction;
            States = states;
        }

        private int[] OperationPrediction { get; }
        private int States { get; }

        public void CreateTransitionMatrix()
        {
            var transition = new double[States, States];

            var prevState = 0;

            for (var i = 0; i < OperationPrediction.Length; i++)
            {
                if (i == 0)
                {
                    prevState = OperationPrediction[i] - 1;
                    continue;
                }

                var state = OperationPrediction[i] - 1;
                transition[prevState, state] += 1;
                prevState = state;
            }

            var newTransition = transition.ToJagged();
            foreach (var row in newTransition)
            {
                row.Normalize(true);
            }

            var emission = transition.ToJagged();
            var count = 0;
            foreach (var row in emission)
            {
                emission[count] = row.Select(element => (element + 1) / (row.Sum() + 2)).ToArray();
                count++;
            }

            var sumOfFirstColumn = emission[0].Sum();
            Logger.Info("Sum of elements in first column: {}", sumOfFirstColumn);
        }
    }
}