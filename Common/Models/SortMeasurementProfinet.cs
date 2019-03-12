using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class SortMeasurementProfinet
    {

        public List<MeasuredVaribles> Measurements { get; set; }
        public Dictionary<int, List<MeasuredVaribles>> Dictionary = new Dictionary<int, List<MeasuredVaribles>>();
        private readonly HashSet<int> _programNumber = new HashSet<int>();

        public SortMeasurementProfinet()
        {
            Measurements = new List<MeasuredVaribles>();            
        }

        public void AddToList(MeasuredVaribles variable)
        {
            Measurements.Add(variable);
        }

        private void SortList()
        {
            foreach (var measurement in Measurements)
            { 
                var programNumber = measurement.ProgramNumber;
                if (!_programNumber.Contains(programNumber))
                {
                    Dictionary.Add(programNumber, new List<MeasuredVaribles>());
                    _programNumber.Add(programNumber);
                }
                Dictionary[programNumber].Add(measurement);
            }
        }

    }
}
