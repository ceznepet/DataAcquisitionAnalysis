using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accord.IO;
using Accord.Math;
using Accord.Statistics.Kernels;
using Common.Models;
using NLog;

namespace Common.Loaders
{

    public class MatLoaders
    {
        private static readonly Logger Logger = LogManager.GetLogger("MatLoaders");

        public static Dictionary<int, List<double[]>> LoadPrograms(string path, int take, int skip, bool product)
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
            return ToDictionary(files.Select(file => LoadToTimeSeries(file.FullName, product)).SelectMany(lists => lists), take, skip, product);
        }

        public static IEnumerable<Operation> LoadProgramsAsTimeSeries(string path, bool product, int take)
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

                return files.Select(file => LoadToTimeSeriesArray(file.FullName, product, take));
        }

        private static List<TimeSeries> LoadToTimeSeries(string fileName, bool product)
        {
            var matReader = new MatReader(fileName);
            var names = matReader.FieldNames;
            var data = matReader.Read<double[,]>(names[0]).ToJagged(true);


            var name = product ? Path.GetFileNameWithoutExtension(fileName)
                               : Path.GetFileName(Path.GetDirectoryName(fileName));
            
            return data.Select(row => new TimeSeries(row, name)).ToList();
        }

        private static Operation LoadToTimeSeriesArray(string fileName, bool product, int take)
        {
            var matReader = new MatReader(fileName);
            var names = matReader.FieldNames;
            var data = matReader.Read<double[,]>(names[0]).ToJagged(true);


            var name = product ? ((int)data[0].ElementAt(data[0].Length - 1)).ToString()
                : Path.GetFileName(Path.GetDirectoryName(fileName));

            var saveData = product ? data.Select(row => row.Take(take).ToArray()).ToArray() : data;
            return new Operation(saveData, name, fileName);
        }


        private static Dictionary<int, List<double[]>> ToDictionary(IEnumerable<TimeSeries> series, int take, int skip, bool product)
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

                var addData = product ? serie.Data.Take(2).Concat(serie.Data.Skip(skip).Take(take)).ToArray()
                                      : serie.Data.Skip(skip).Take(take).ToArray();

                dictionary[opCounter].Add(addData);
            }

            return dictionary;
        }       
    }
}