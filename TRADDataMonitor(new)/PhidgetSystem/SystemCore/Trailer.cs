﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SystemCore
{
    public class Trailer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DataMonitoringDevice> Devices { get; set; }
    }
}
