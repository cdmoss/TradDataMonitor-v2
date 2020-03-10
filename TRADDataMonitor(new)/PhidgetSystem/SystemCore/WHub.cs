using Phidget22;
using System;
using System.Collections.Generic;
using System.Text;

namespace SystemCore
{
    // this class wraps the Phidget22.Hub type
    public class WHub : DataMonitoringDevice
    {
        ////public Hub Hub { get; set; }
        //public int ID { get; set; }
        //public string Name { get; set; }
        //public List<Sensor> Sensors { get; set; }

        public WHub(Hub hub)
        {
            ID = hub.DeviceSerialNumber;
        }
    }
}
