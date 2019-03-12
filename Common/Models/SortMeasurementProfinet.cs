using System;
using System.Collections.Generic;
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
        }

    }
}
