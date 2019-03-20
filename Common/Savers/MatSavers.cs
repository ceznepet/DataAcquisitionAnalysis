using System.Collections.Generic;
using Accord.Math;
using csmatio.io;
using csmatio.types;

namespace Common.Savers
{
    public class MatSavers
    {
        public static void ToMatFile(List<double[]> measuredData, string name, string fileName)
        {
            var mMatrix = new MLDouble("Operation_" + name, measuredData.ToArray().Transpose());
            var mList = new List<MLArray>();
            mList.Add(mMatrix);
            var fill = int.Parse(name) < 10 ? "000" : "00";

            var mFileWrite = new MatFileWriter(fileName + "_op_" + fill + name + ".mat", mList, false);
        }
    }
}
