using System;
using System.Collections.Generic;
using System.Text;

namespace SystemCore
{
    public class Trailer
    {
        //List<Hub> Hubs;
        List<HubSensor> HubSensors;
        Dictionary<Tuple<int, int>, SensorType> ConfigManifest;
    }
}
