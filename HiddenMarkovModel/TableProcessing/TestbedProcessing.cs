using System.Data;
using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Linear;
using HiddenMarkovModel.Loaders;

namespace HiddenMarkovModel.TableProcessing
{
    public class TestbedProcessing
    {

        private int ColumnSize { get; set; } = 0;
        private DataTable DataT { get; set; }
        private PrincipalComponentAnalysis Pca { get; set; }
        private MultivariateLinearRegression Transform { get; set; }
        public double[][] OriginalData { get; private set; }
        public double[][] ReduceData { get; private set; }


        public TestbedProcessing(string inputFolder, int reduce, bool raw)
        {

            DataT = Loader.LoadCSV(inputFolder);
            //Proces();
            //if (raw == false)
            //{
            //    ReduceDimension(reduce);
            //    var learner = new LearnModel(ReduceData, reduce);
            //}
            //else
            //{
            //    var learner = new LearnModel(OriginalData, ColumnSize - 2);

            //}
        }

        //private void Proces()
        //{
        //    var sizeRow = DataT.Rows.Count;
        //    ColumnSize = DataT.Columns.Count;
        //    double[][] data = new double[sizeRow][];
        //    var list = DataT.AsEnumerable().ToList();
        //    var i = 0;
        //    foreach (var row in list)
        //    {
        //        data[i] = ProcesRow(row);
        //        i++;
        //    }
        //    OriginalData = data;
        //}


        //private double[] ProcesRow(DataRow row)
        //{
        //    var j = 0;
        //    var data = new double[ColumnSize - 2];
        //    foreach (var column in DataT.Columns)
        //    {
        //        if (j > 1)
        //        {
        //            data[j - 2] = row.String2Double(column.ToString());
        //        }
        //        j++;
        //    }
        //    return data;
        //}

        private void ReduceDimension(int dimension)
        {

            Pca = new PrincipalComponentAnalysis()
            {
                Method = PrincipalComponentMethod.Standardize,
                Whiten = false
            };

            Transform = Pca.Learn(OriginalData);

            Pca.NumberOfOutputs = dimension;

            ReduceData = Pca.Transform(OriginalData);
        }
    }
}