using Common.Models;
using System;
using System.Collections.Generic;
using NLog;
using NLog.Fluent;


namespace Common.Extensions
{
    public static class VaribaleComponentExtensions
    {
        public static DateTime ToDateTime(this List<VariableComponent> components)
        {
            if(components.Count != 7)
            {
                return new DateTime();
            }
            return new DateTime((int)components[0].Value,  //Year
                                (int)components[1].Value,  //Month
                                (int)components[2].Value,  //Day
                                (int)components[3].Value,  //Hours
                                (int)components[4].Value,  //Minutes
                                (int)components[5].Value,  //Seconds
                                (int)components[6].Value); //Milliseconds
        }
    }
}
