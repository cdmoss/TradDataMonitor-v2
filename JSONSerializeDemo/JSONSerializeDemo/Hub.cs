using System;
using System.Collections.Generic;
using System.Text;

namespace JSONSerializeDemo
{
    public class Hub
    {
        public int SerialNumber { get; set; }
        public string HubName { get; set; }
        public List<Sensor> Sensors { get; set; }
    }
}
