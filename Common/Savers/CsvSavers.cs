using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Common.Models.Tcp;
using Common.Models.Measurement;

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

        public static void CsvHeader(string fileName)
        {
            var header = "Save Time,Robot Time,Operation,Position A1,Position A2,Position A3,Position A4,Position A5,Position A6,Velocity A1,Velocity A2,Velocity A3,Velocity A4,Velocity A5,Velocity A6,Current A1,Current A2,Current A3,Current A4,Current A5,Current A6,Temp A1,Temp A2,Temp A3,Temp A4,Temp A5,Temp A6,Torque A1,Torque A2,Torque A3,Torque A4,Torque A5,Torque A6\n";
            File.AppendAllText(fileName + ".csv", header);
        }

        public static void ToCsvFile(MeasuredVariables measuredData, string fileName)
        {
            var dateTime = measuredData.SaveTime;
            var robotTime = measuredData.RobotTime;
            var prNumber = measuredData.ProgramNumber.ToString();
            var begin_1 = string.Join(", ", dateTime, robotTime);
            var begin = string.Join(", ", begin_1, prNumber);
            var newLine = string.Join(", ",
                measuredData.GetMeasuredValues().ToArray()
                    .Select(element => element.ToString(CultureInfo.InvariantCulture)).ToArray());
            File.AppendAllText(fileName + ".csv", begin + ", " + newLine + "\n");
        }


        public static void ToCsvFile(List<double[]> measuredData, string name, string fileName)
        {
            var fill = int.Parse(name) < 10 ? "000" : int.Parse(name) < 100 ? "00" : "0";
            foreach(var row in measuredData)
            {
                File.AppendAllText(fileName + "_op_" + fill + name + ".csv", string.Join(",", row)+ "\n");
            }
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

        public static void SaveMarkovPath(string fileName, int[] path)
        {
            File.AppendAllText(fileName, "Operation\n");

            foreach(var op in path)
            {
                var line = op.ToString();
                File.AppendAllText(fileName, line + "\n");
            }

        }
    }
}