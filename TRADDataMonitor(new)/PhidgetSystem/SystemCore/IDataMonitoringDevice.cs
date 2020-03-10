using System;
using System.Collections.Generic;
using System.Text;

namespace SystemCore
{
    public interface IDataMonitoringDevice
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Sensor> Sensors { get; set; }
    }
}
