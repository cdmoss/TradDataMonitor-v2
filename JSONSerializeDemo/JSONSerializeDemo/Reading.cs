using System;
using System.Collections.Generic;
using System.Text;

namespace JSONSerializeDemo
{
    public class Reading
    {
        public int SerialNumber { get; set; }
        public double Value { get; set; }
        public UnitOfMeasure Unit { get; set; }
        public DateTime TimeOfReading { get; set; }
    }
}
