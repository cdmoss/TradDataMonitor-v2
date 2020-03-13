using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SystemCore;

namespace PiApp
{
    public static class ConfigurationManager
    {
        public static Trailer GetConfig()
        {
            string config = File.ReadAllText("config.json");
            return JsonConvert.DeserializeObject<Trailer>(config);
        }
    }
}
