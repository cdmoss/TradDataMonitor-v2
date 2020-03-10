using System;
using System.Collections.Generic;
using System.Text;

namespace SystemCore
{
    public class Sensor
    {
        //public int SerialNumber { get; set; }
        public SensorType SensorType { get; set; }
        public int HubPort { get; set; }
    }
}
