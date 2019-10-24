﻿using System;
using System.Collections.Generic;

namespace Common.Models.Measurement
{
    public class MeasuredVariables
    {
        public string RobotTime { get; set; }
        public string SaveTime { get; set; }
        public int ProgramNumber { get; set; }
        public List<Variable> Variables = new List<Variable>();

        public IEnumerable<double> GetMeasuredValues()
        {
            foreach (var variable in Variables)
            {
                foreach (var value in variable.GetValues())
                {
                    yield return value;
                }
            }
        }
    }
}