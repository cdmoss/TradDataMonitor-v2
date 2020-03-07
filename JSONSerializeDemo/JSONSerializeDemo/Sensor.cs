using Phidget22;
using System;
using System.Collections.Generic;
using System.Text;

namespace JSONSerializeDemo
{
    public class Sensor : Phidget
    {
        public SensorType TypeOfSensor { get; set; }
    }
}
