using Phidget22;
using System;
using System.Collections.Generic;
using System.Text;
using SystemCore;

namespace PiApp
{
    public class SensorManager
    {
        private Trailer trailer;
        private List<Hub> Hubs = new List<Hub>();
        private List<Phidget> Channels = new List<Phidget>();

        public SensorManager(Trailer t)
        {
            trailer = t;

            // iterate through trailer, create appropriate channels
            foreach (DataMonitoringDevice dmd in t.Devices)
            {
                if (dmd is WHub)
                {
                    Hub hub = new Hub()
                    {
                        DeviceLabel = dmd.Name,
                        DeviceSerialNumber = dmd.ID
                    };

                    Hubs.Add(hub);
                    foreach (Sensor sensor in dmd.Sensors)
                    {
                        switch (sensor.SensorType)
                        {
                            case SensorType.Humidity:
                                HumiditySensor hs = new HumiditySensor()
                                {
                                    HubPort = sensor.HubPort,
                                    DeviceSerialNumber = dmd.ID
                                };

                                Channels.Add(hs);

                                break;
                            case SensorType.AirTemperature:
                                TemperatureSensor ats = new TemperatureSensor()
                                {
                                    HubPort = sensor.HubPort,
                                    DeviceSerialNumber = dmd.ID
                                };

                                Channels.Add(ats);

                                break;
                            case SensorType.SoilTemperature:
                                TemperatureSensor sts = new TemperatureSensor()
                                {
                                    HubPort = sensor.HubPort,
                                    DeviceSerialNumber = dmd.ID
                                };

                                Channels.Add(sts);
                                break;
                            case SensorType.Oxygen:
                                VoltageInput oxygen = new VoltageInput()
                                {
                                    HubPort = sensor.HubPort,
                                    DeviceSerialNumber = dmd.ID
                                };

                                Channels.Add(oxygen);
                                break;
                            case SensorType.Moisture:
                                VoltageInput moisture = new VoltageInput()
                                {
                                    HubPort = sensor.HubPort,
                                    DeviceSerialNumber = dmd.ID
                                };

                                Channels.Add(moisture);
                                break;
                        }
                    }
                }
                if (dmd is CCS811Sensor)
                {
                    // handle voc sensors
                }
            }

            foreach (Hub hub in Hubs)
            {
                hub.Attach += Hub_Attach;
                hub.Detach += Hub_Detach; ;

                hub.Open();
            }

            foreach (Phidget channel in Channels)
            {
                channel.Attach += Channel_Attach;
                channel.Detach += Channel_Detach;
                
                channel.Open();
            }
        }

        private void Hub_Attach(object sender, Phidget22.Events.AttachEventArgs e)
        {
            Console.WriteLine($"Hub {((Hub)sender).DeviceSerialNumber} was attached");
        }

        private void Hub_Detach(object sender, Phidget22.Events.DetachEventArgs e)
        {
            Console.WriteLine($"Hub {((Hub)sender).DeviceSerialNumber}was detached");
        }

        private void Channel_Attach(object sender, Phidget22.Events.AttachEventArgs e)
        {
            Console.WriteLine($"Hub {((Phidget)sender).DeviceSerialNumber} was attached");
        }

        private void Channel_Detach(object sender, Phidget22.Events.DetachEventArgs e)
        {
            Console.WriteLine($"Hub {((Hub)sender).DeviceSerialNumber} was detached");
        }

        // public List<Reading> GetAllData()
        // {
        //     foreach (var item in collection)
        //     {
        //
        //     }
        // } 
    }
}
