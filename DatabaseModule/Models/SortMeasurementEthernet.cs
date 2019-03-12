using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseModule.Models
{
    public class SortMeasurementEthernet
    {

        public List<TcpRobot> Measurements { get; set; }
        public Dictionary<int, List<TcpRobot>> Dictionary = new Dictionary<int, List<TcpRobot>>();
        private readonly HashSet<int> _programNumber = new HashSet<int>();

        public SortMeasurementEthernet()
        {
            Measurements = new List<TcpRobot>();            
        }

        public void AddToList(TcpRobot variable)
        {
            Measurements.Add(variable);
        }

        public void SortList()
        {
            foreach (var measurement in Measurements)
            { 
                var programNumber = (int)measurement.ProgramNumber.Value;
                if (!_programNumber.Contains(programNumber))
                {
                    Dictionary.Add(programNumber, new List<TcpRobot>());
                    _programNumber.Add(programNumber);
                }
                Dictionary[programNumber].Add(measurement);
            }
        }

    }
}
