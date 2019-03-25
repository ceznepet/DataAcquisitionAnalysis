﻿using System;
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
            File.AppendAllText(fileName + ".csv", begin + ", " + newLine);
        }

        public static void ToCsvFile(MeasuredVariables measuredData, string fileName)
        {
            var dateTime = measuredData.SaveTime;
            var prNumber = measuredData.ProgramNumber.ToString();
            var begin = string.Join(", ", dateTime, prNumber);
            var newLine = string.Join(", ",
                measuredData.GetMeasuredValues().ToArray()
                    .Select(element => element.ToString(CultureInfo.InvariantCulture)).ToArray());
            File.AppendAllText(fileName + ".csv", begin + ", " + newLine);
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
    }
}