using Common.Models;
using KunbusRevolutionPiModule.Kunbus;
using System;
using System.Collections.Generic;

namespace KunbusRevolutionPiModule.Robot
{
    public class RobotTime
    {       
        public List<VariableComponent> CurrentTime = new List<VariableComponent>();

        public RobotTime(List<VariableComponent> Time)
        {
            CurrentTime = Time;
        }

        public DateTime ToDataTime()
        {
            return new DateTime((int)CurrentTime[0].Value,  //Year
                                (int)CurrentTime[1].Value,  //Month
                                (int)CurrentTime[2].Value,  //Day
                                (int)CurrentTime[3].Value,  //Hours
                                (int)CurrentTime[4].Value,  //Minutes
                                (int)CurrentTime[5].Value,  //Seconds
                                (int)CurrentTime[6].Value); //Milliseconds
        }
    }
}
