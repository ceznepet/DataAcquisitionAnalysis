using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NLog;

namespace Common.Loaders
{
    public class CsvLoaders
    {
        private static readonly Logger Logger = LogManager.GetLogger("CsvLoaders");

        public static DataTable LoadCSV(string strFilePath)
        {
            Logger.Info("Start loading data...");
            using (var streamReader = new StreamReader(strFilePath))
            {
                var headers = streamReader.ReadLine()?.Split(',');
                var dataTable = new DataTable();
                var i = 1;
                foreach (var header in headers)
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
                    for (i = 0; i < headers.Length; i++) dataRow[i] = rows[i];

                    dataTable.Rows.Add(dataRow);
                }

                Logger.Info("Loading is succesfully done...");
                return dataTable;
            }
        }
    }
}
