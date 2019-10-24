﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class KunbusSetup
    {
        public string ConfigurationFile { get; set; }
        public bool BigEndian { get; set; }
        public string DatabaseLocation { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseDocument { get; set; }
        public int ReadingPerios { get; set; }
    }
}
