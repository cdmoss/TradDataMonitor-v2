using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Phidget22;

namespace JSONSerializeDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Tuple<string, int, string> test = { }

            List<Hub> hubs;
            using (StreamReader reader = new StreamReader("../../../config.json"))
            {
                string jsonTest = reader.ReadToEnd();
                hubs = JsonConvert.DeserializeObject<List<Hub>>(jsonTest);
            }

            foreach (Hub hub in hubs)
            {
                Phidget22.Hub h = new Phidget22.Hub();
                h.
                foreach (Sensor sensor in hub.Sensors)
                {
                    switch (sensor.TypeOfSensor)
                    {
                        case "Humidity":
                            sensor = new HumiditySensor();
                            TemperatureSensor ats = new TemperatureSensor();
                            hs.HubPort = sensor.Port;
                            ats.HubPort = sensor.Port;
                            break;

                        case "Soil Temperature":
                            TemperatureSensor sts = new TemperatureSensor();
                            sts.HubPort = sensor.Port;
                            break;
                        case "Soil Moisture":
                            VoltageInput vi = new VoltageInput();
                            vi.HubPort = sensor.Port;
                            break;
                        case "Oxygen":
                            VoltageInput os = new VoltageInput();
                            os.HubPort = sensor.Port;
                            break;
                        case "VOC":
                            break;
                    }
                }
            }

            Console.ReadKey();
        }
    }
}
