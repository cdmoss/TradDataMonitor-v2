using System;
using System.Collections.Generic;
using System.Text;

namespace SystemCore
{
    public class Reading
    {
        public int SerialNumber { get; set; }
        public double Value { get; set; }
        public SensorType Unit { get; set; }
        public DateTime TimeOfReading { get; set; }
    }
}
