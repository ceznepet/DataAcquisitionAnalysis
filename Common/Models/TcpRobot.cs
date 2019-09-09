using System.Collections.Generic;

namespace Common.Models
{
    public class TcpRobot
    {
        public List<RobotAxes> Measurement = new List<RobotAxes>();
        public RobotAxes Position { get; set; }
        public RobotAxes Velocity { get; set; }
        public RobotAxes Current { get; set; }
        public RobotAxes Temp { get; set; }
        public RobotAxes Torque { get; set; }
        public RobotTime Time { get; set; }
        public ProgramNumber ProgramNumber { get; set; }
        public bool Called = false;

        public void ToList()
        {
            Measurement.Add(Position);
            Measurement.Add(Velocity);
            Measurement.Add(Current);
            Measurement.Add(Temp);
            Measurement.Add(Torque);
        }

        public IEnumerable<double> FilePreparation()
        {
            foreach (var data in Measurement)
            {
                if (!Called)
                {
                    data.CreateList();
                }
                foreach (var axis in data.Axis) yield return axis;
            }
        }
    }

    public class ProgramNumber
    {
        public double Value { get; set; }
    }
}