using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Accord.IO;
using Accord.Math;
using HMModel.Models.Data;
using NLog;

namespace HMModel.Loaders
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

        public static Dictionary<int, List<double[]>> LoadPrograms(string path, int take)
        {
            var directories = new List<DirectoryInfo>();
            var files = new List<FileInfo>();

            var rootDir = new DirectoryInfo(path);
            var subDirectories = rootDir.GetDirectories().ToList();

            if (subDirectories.Count > 0)
            {
                directories.AddRange(subDirectories);
            }
            else
            {
                directories.Add(rootDir);
            }

            foreach (var dir in directories)
            {
                files.AddRange(dir.GetFiles("*.mat"));
            }

            if (files.Count == 0)
            {
                throw new Exception("There are no *.mat files in that directory");
            }
            return ToDictionary(files.Select(file => LoadDataFromMat(file.FullName, take)).SelectMany(lists => lists), take);
        }

        private static List<TimeSeries> LoadDataFromMat(string fileName, int take)
        {
            var matReader = new MatReader(fileName);
            var names = matReader.FieldNames;
            var data = matReader.Read<double[,]>(names[0]).ToJagged(true);
            return data.Select(row => new TimeSeries(row, Path.GetFileName(Path.GetDirectoryName(fileName)))).ToList();
        }

        private static Dictionary<int, List<double[]>> ToDictionary(IEnumerable<TimeSeries> series, int take)
        {
            var programs = new HashSet<string>();
            var dictionary = new Dictionary<int, List<double[]>>();
            var opCounter = 0;
            foreach (var serie in series)
            {
                var name = serie.Name;
                if (!programs.Contains(name))
                {
                    opCounter++;
                    dictionary.Add(opCounter, new List<double[]>());
                    programs.Add(name);
                }

                dictionary[opCounter].Add(serie.Data.Take(take).ToArray());
            }

            return dictionary;
        }
    }
}