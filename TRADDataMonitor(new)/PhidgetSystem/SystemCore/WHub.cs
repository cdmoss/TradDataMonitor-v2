using Phidget22;
using System;
using System.Collections.Generic;
using System.Text;

namespace SystemCore
{
    // this class wraps the Phidget22.Hub type
    public class WHub
    {
        public Hub Hub { get; set; }
        public int SerialNumber { get; set; }
        public List<HubSensor> Sensors { get; set; }

        public WHub(Hub hub)
        {
            Hub = hub;
            SerialNumber = hub.DeviceSerialNumber;
        }
    }
}
