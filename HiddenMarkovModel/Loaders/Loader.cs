using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using NLog;

namespace HiddenMarkovModel.Loaders
{
    public class Loader
    {
        private static readonly Logger Logger = LogManager.GetLogger("Loader");
        public static DataTable LoadCSV(string strFilePath)
        {
            Logger.Info("Start loading data...");
            using (var streamReader = new StreamReader(strFilePath))
            {
                var headers = streamReader.ReadLine()?.Split(',');
                var dataTable = new DataTable();
                int i = 1;
                foreach (string header in headers)
                {
                    dataTable.Columns.Add(i.ToString());
                    i++;
                }

                while (!streamReader.EndOfStream)
                {
                    var rows = Regex.Split(streamReader.ReadLine() ??
                                           throw new InvalidOperationException(),
                        ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                    var dataRow = dataTable.NewRow();
                    for (i = 0; i < headers.Length; i++)
                    {
                        dataRow[i] = rows[i];
                    }

                    dataTable.Rows.Add(dataRow);
                }
                Logger.Info("Loading is succesfully done...");
                return dataTable;
            }
        }

        public static void LoadPrograms(string path)
        {

        }

    }
}