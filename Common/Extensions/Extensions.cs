using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Models;

namespace Common.Extensions
{
    public static class Extensions
    {
        public static double[][][] ToSequence(this List<Operation> data)
        {
            return data.Select(element => element.Data).ToArray();
        }

        public static double[] Shape(this double[][] array)
        {
            var dimension_x = array.Length;
            var dimension_y = array[0].Length;

            var shape = new double[] {dimension_x, dimension_y};

            return shape;
        }

        public static double[][][] ToSequence(this Dictionary<int, List<double[]>> data)
        {
            var length = 22;
            var sequences = new double[length + 1][][];
            for (var i = 1; i <= length; i++)
            {
                sequences[i - 1] = data[i].ToArray();
            }

            return sequences;
        }

        public static int[] GetLabels()
        {
            var length = 22;
            var labels = new int[length + 1];
            for (var i = 1; i <= length; i++)
            {
                labels[i - 1] = i;
            }

            return labels;
        }

        public static int[] GetLabels(this List<Operation> data)
        {
            return data.Select(element => int.Parse(element.Name)).ToArray();

        }
    }
}
