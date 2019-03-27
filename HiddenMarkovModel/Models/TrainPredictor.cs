using System.Runtime.InteropServices.ComTypes;
using Accord.Math;


namespace HMModel.Models
{
    public class TrainPredictor
    {
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
        }
    }
}