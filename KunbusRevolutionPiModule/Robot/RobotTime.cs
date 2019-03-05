using KunbusRevolutionPiModule.Kunbus;
using System;
using System.Collections.Generic;

namespace KunbusRevolutionPiModule.Robot
{
    public class RobotTime
    {
        // $DATE
        public KunbusIOData Millisecond = new KunbusIOData(24, "Millisecond");
        public KunbusIOData Second = new KunbusIOData(20, "Second");
        public KunbusIOData Minute = new KunbusIOData(16, "Minute");
        public KunbusIOData Hour = new KunbusIOData(12, "Hours");
        public KunbusIOData Day = new KunbusIOData(8, "Day");
        public KunbusIOData Month = new KunbusIOData(4, "Month");
        public KunbusIOData Year = new KunbusIOData(0, "Year");

        public List<KunbusIOData> CurrentTime = new List<KunbusIOData>();

        public RobotTime()
        {
            CurrentTime.Add(Year);
            CurrentTime.Add(Month);
            CurrentTime.Add(Day);;
            CurrentTime.Add(Hour);
            CurrentTime.Add(Minute);
            CurrentTime.Add(Second);
            CurrentTime.Add(Millisecond);
        }

        public DateTime ToDataTime()
        {
            return new DateTime((int)CurrentTime[0].Value,
                                (int)CurrentTime[1].Value, 
                                (int)CurrentTime[2].Value,  
                                (int)CurrentTime[3].Value, 
                                (int)CurrentTime[4].Value, 
                                (int)CurrentTime[5].Value, 
                                (int)CurrentTime[6].Value);
        }
    }
}
