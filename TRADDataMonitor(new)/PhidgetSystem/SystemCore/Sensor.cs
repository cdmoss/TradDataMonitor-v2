using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Phidget22;

namespace SystemCore
{
    public class Sensor
    {
        public int SerialNumber { get; set; }
        [JsonProperty("SensorType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SensorType SensorType { get; set; }
        public int HubPort { get; set; }
        public Phidget Channel { get; set; }
    }
}
