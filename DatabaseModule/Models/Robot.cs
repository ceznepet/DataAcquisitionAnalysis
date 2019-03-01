using System.Collections.Generic;

namespace DatabaseModule.Models
{
    public class Robot
    {
        public List<RobotAxes> Measurement = new List<RobotAxes>();
        public RobotAxes Position { get; set; }
        public RobotAxes Velocity { get; set; }
        public RobotAxes Current { get; set; }

        public void ToList()
        {
            Measurement.Add(Position);
            Measurement.Add(Velocity);
            Measurement.Add(Current);
        }

        public IEnumerable<double> FilePreparation()
        {
            ToList();
            foreach (var data in Measurement)
            {
                data.CreateList();
                foreach (var axis in data.Axis) yield return axis;
            }
        }
    }
}