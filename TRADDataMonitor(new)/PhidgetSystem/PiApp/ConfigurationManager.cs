using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using SystemCore;

namespace PiApp
{
    public static class ConfigurationManager
    {
        public static Trailer GetConfig()
        {
            return JsonConvert.DeserializeObject<Trailer>("../../../config.json");
        }
    }
}
