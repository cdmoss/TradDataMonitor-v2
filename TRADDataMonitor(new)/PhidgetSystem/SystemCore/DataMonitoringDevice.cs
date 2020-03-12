using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SystemCore
{
    public class DataMonitoringDevice
    {
        public int ID { get; set; }
        public string Name { get; set; }
        [JsonProperty("DeviceType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceType DeviceType { get; set; }
        public List<Sensor> Sensors { get; set; }
    }
}
