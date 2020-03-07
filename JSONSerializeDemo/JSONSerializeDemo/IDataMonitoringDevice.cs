using System;
using System.Collections.Generic;
using System.Text;
using Phidget22;

namespace JSONSerializeDemo
{
    public interface IDataMonitoringDevice
    {
        string Name { get; set; }
        List<Reading> Readings();
    }
}
