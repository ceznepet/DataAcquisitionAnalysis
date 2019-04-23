using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Common.Models;

namespace Common.Savers
{
    public static class CsvSavers
    {
        public static void ToCsvFile(TcpRobot measuredData, string fileName)
        {
            var dateTime = measuredData.Time.GetDate().ToString("yyyy-MM-dd-HH-mm-ss-FFF");
            var prNumber = ((int) measuredData.ProgramNumber.Value).ToString();
            var begin = string.Join(", ", dateTime, prNumber);
            var newLine = string.Join(", ",
                measuredData.FilePreparation().ToArray()
                    .Select(element => element.ToString(CultureInfo.InvariantCulture)).ToArray());
            File.AppendAllText(fileName + ".csv", begin + ", " + newLine + "\n");
        }

        public static void ToCsvFile(MeasuredVariables measuredData, string fileName)
        {
            var dateTime = measuredData.SaveTime;
            var prNumber = measuredData.ProgramNumber.ToString();
            var begin = string.Join(", ", dateTime, prNumber);
            var newLine = string.Join(", ",
                measuredData.GetMeasuredValues().ToArray()
                    .Select(element => element.ToString(CultureInfo.InvariantCulture)).ToArray());
            File.AppendAllText(fileName + ".csv", begin + ", " + newLine + "\n");
        }

        public static void ToCsvFile(double[] measuredData, int operationNumber, string fileName)
        {
            var prNumber = operationNumber.ToString();
            var time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var newLine = string.Join(", ", measuredData
                          .Select(element => element.ToString(CultureInfo.InvariantCulture))
                          .ToArray());
            File.AppendAllText(fileName + ".csv", time + ", " + prNumber + ", " + newLine + "\n");
        }

        public static void ToCsvFile(string measuredData, string fileName)
        {
            File.AppendAllText(fileName + ".csv",  measuredData + "\n");
        }

        public static void SaveClassificationOuput(Dictionary<int, double>  testOutput, string columns, string fileName)
        {
            File.WriteAllText(fileName, columns + "\n");
            foreach(var key in testOutput.Keys)
            {
                var test = testOutput[key].ToString("0.0000000", CultureInfo.InvariantCulture);

                File.AppendAllText(fileName, key + "," +test + "\n");
            }
        }

        public static void SaveLogLikelihoodEvaluation(string fileName, double[][] logLikelihood)
        {
            File.AppendAllText(fileName, "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22\n");
            foreach (var likelihood in logLikelihood)
            {
                var line = string.Join(", ", likelihood
                                 .Select(element => element.ToString("0.0000000", CultureInfo.InvariantCulture))
                                 .ToArray());
                File.AppendAllText(fileName, line + "\n");
            }

        }
    }
}