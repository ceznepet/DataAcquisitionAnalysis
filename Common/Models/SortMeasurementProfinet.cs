using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Models
{
    public class SortMeasurementProfinet
    {

        public List<MeasuredVariables> Measurements { get; set; }
        public Dictionary<int, List<MeasuredVariables>> Dictionary = new Dictionary<int, List<MeasuredVariables>>();
        private readonly HashSet<int> _programNumber = new HashSet<int>();

        public SortMeasurementProfinet()
        {
            Measurements = new List<MeasuredVariables>();            
        }

        public void AddToList(MeasuredVariables variable)
        {
            Measurements.Add(variable);
        }

        public void SortByTime()
        {
            var result = Measurements.AsParallel().AsOrdered().OrderBy(measurement => measurement.SaveTime);
        }

        public void SortList()
        {
            foreach (var measurement in Measurements)
            { 
                var programNumber = measurement.ProgramNumber;
                if (!_programNumber.Contains(programNumber))
                {
                    Dictionary.Add(programNumber, new List<MeasuredVariables>());
                    _programNumber.Add(programNumber);
                }
                Dictionary[programNumber].Add(measurement);
            }
            SortTime();
        }

        private void SortTime()
        {
            foreach (var key in Dictionary.Keys)
            {
                Dictionary[key].Sort((x, y) => String.Compare(x.SaveTime, y.SaveTime, StringComparison.Ordinal));
            }
        }

        public void SortMeasurement()
        {
            Measurements.Sort((x, y) => String.Compare(x.SaveTime, y.SaveTime, StringComparison.Ordinal));
        }

        public void SortByProduct()
        {
            var count = 0;
            var hit = false;
            Dictionary.Add(count, new List<MeasuredVariables>());
            foreach (var measurement in Measurements)
            {
                var programNumber = measurement.ProgramNumber;
                if (programNumber % 22 == 0 && !hit)
                {
                    count++;
                    Dictionary.Add(count, new List<MeasuredVariables>());
                    hit = true;
                }

                if (programNumber == 1 && hit)
                {
                    hit = false;
                }
                Dictionary[count].Add(measurement);
            }
            SortTime();
        }
    }
}
