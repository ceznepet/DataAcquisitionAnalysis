using System;

namespace Common.Models
{
    public class RobotTime
    {
        public double Year { get; set; }
        public double Month { get; set; }
        public double Day { get; set; }
        public double Hour { get; set; }
        public double Minute { get; set; }
        public double Second { get; set; }
        public double Millisecond { get; set; }

        public DateTime GetDate()
        {
            return new DateTime((int)Year, (int)Month, (int)Day, (int)Hour, (int)Minute, (int)Second, (int)Millisecond);
        }
    }
}
