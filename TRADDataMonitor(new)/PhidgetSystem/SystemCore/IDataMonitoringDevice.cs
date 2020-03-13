using System;
using System.Collections.Generic;
using System.Text;

namespace SystemCore
{
    public interface IDataMonitoringDevice
    {
        int ID { get; set; }
        string Name { get; set; }
        List<Sensor> Sensors { get; set; }
    }
}
