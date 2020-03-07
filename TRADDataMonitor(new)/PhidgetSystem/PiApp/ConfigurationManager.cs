using Phidget22;
using System;
using System.Collections.Generic;
using System.Text;
using SystemCore;

namespace PiApp
{
    // deserializes JSON config data sent from central server, serializes and sends current JSON config to central server when requested
    public class ConfigurationManager
    {
        List<Hub> Hubs;
        List<HubSensor> HubSensors;
        Dictionary<Tuple<int, int>, SensorType> ConfigManifest;

        public ConfigurationManager()
        {

        }
    }
}
