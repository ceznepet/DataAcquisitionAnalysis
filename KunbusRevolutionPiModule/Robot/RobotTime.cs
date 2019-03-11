using Common.Models;
using KunbusRevolutionPiModule.Kunbus;
using System;
using System.Collections.Generic;

namespace KunbusRevolutionPiModule.Robot
{
    public class RobotTime
    {
        // $DATE
        public VariableComponent Millisecond = new VariableComponent(24, "Millisecond");
        public VariableComponent Second = new VariableComponent(20, "Second");
        public VariableComponent Minute = new VariableComponent(16, "Minute");
        public VariableComponent Hour = new VariableComponent(12, "Hours");
        public VariableComponent Day = new VariableComponent(8, "Day");
        public VariableComponent Month = new VariableComponent(4, "Month");
        public VariableComponent Year = new VariableComponent(0, "Year");

        public List<VariableComponent> CurrentTime = new List<VariableComponent>();

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
