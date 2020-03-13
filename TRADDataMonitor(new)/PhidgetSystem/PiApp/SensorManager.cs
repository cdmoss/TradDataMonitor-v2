using Phidget22;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;
using SystemCore;
// Added
using System.Linq;

namespace PiApp
{
    public class SensorManager
    {
        private Trailer trailer;
        // Outdated way of managing the attach events for voltage inputs
        //private bool 
        //    moistureAttached = false, 
        //    oxygenAttached = false;
        private List<Tuple<VoltageInput, bool, SensorType>> voltageInputs = new List<Tuple<VoltageInput, bool, SensorType>>();
        private List<Hub> Hubs = new List<Hub>();
        private List<VITimer> VITimers = new List<VITimer>();

        public SensorManager(Trailer t)
        {
            trailer = t;

            // iterate through trailer, create appropriate channels
            foreach (DataMonitoringDevice dmd in t.Devices)
            {
                if (dmd.DeviceType == DeviceType.PhidgetHub)
                {
                    InitializeHub(dmd);
                    
                    foreach (Sensor sensor in dmd.Sensors)
                    {
                        InitializeSensor(sensor, dmd);
                    }
                }
                if (dmd.DeviceType == DeviceType.CCS811)
                {
                    // handle voc sensors
                }
            }
        }

        private void InitializeHub(DataMonitoringDevice dmd)
        {
            Hub hub = new Hub()
            {
                //DeviceLabel = dmd.Name,
                DeviceSerialNumber = dmd.ID
            };

            hub.Attach += Hub_Attach;
            hub.Detach += Hub_Detach;

            hub.Open();
            Console.WriteLine($"Hub {hub.DeviceSerialNumber} is ready for attachment");

            Hubs.Add(hub);
        }

        private void InitializeSensor(Sensor sensor, DataMonitoringDevice dmd)
        {
            switch (sensor.SensorType)
            {
                case SensorType.Humidity:
                    HumiditySensor hs = new HumiditySensor()
                    {
                        HubPort = sensor.HubPort,
                        DeviceSerialNumber = dmd.ID
                    };

                    sensor.Channel = hs;

                    break;
                case SensorType.AirTemperature:
                    TemperatureSensor ats = new TemperatureSensor()
                    {
                        HubPort = sensor.HubPort,
                        DeviceSerialNumber = dmd.ID
                    };

                    sensor.Channel = ats;

                    break;
                case SensorType.SoilTemperature:
                    TemperatureSensor sts = new TemperatureSensor()
                    {
                        HubPort = sensor.HubPort,
                        DeviceSerialNumber = dmd.ID
                    };

                    sensor.Channel = sts;

                    break;
                case SensorType.Oxygen:
                    VoltageInput oxygen = new VoltageInput()
                    {
                        HubPort = sensor.HubPort,
                        DeviceSerialNumber = dmd.ID,
                        IsHubPortDevice = true
                    };

                    oxygen.VoltageChange += VoltageInput_VoltageChange;

                    sensor.Channel = oxygen;

                    //VITimers.Add(new VITimer(sensor.HubPort));

                    Tuple<VoltageInput, bool, SensorType> tOxy = Tuple.Create(oxygen, false, SensorType.Oxygen);
                    voltageInputs.Add(tOxy);

                    break;
                case SensorType.Moisture:
                    VoltageInput moisture = new VoltageInput()
                    {
                        HubPort = sensor.HubPort,
                        DeviceSerialNumber = dmd.ID,
                        IsHubPortDevice = true
                    };

                    moisture.VoltageChange += VoltageInput_VoltageChange;            

                    sensor.Channel = moisture;

                    //VITimers.Add(new VITimer(sensor.HubPort));

                    Tuple<VoltageInput, bool, SensorType> tMoist = Tuple.Create(moisture, false, SensorType.Moisture);
                    voltageInputs.Add(tMoist);

                    break;
            }

            sensor.Channel.Attach += Channel_Attach;
            sensor.Channel.Detach += Channel_Detach;

            sensor.Channel.Open();

            Console.WriteLine($"{sensor.SensorType} on port {sensor.Channel.HubPort} on hub {sensor.Channel.DeviceSerialNumber} is ready for attachment");
        }

        private void VoltageInput_VoltageChange(object sender, Phidget22.Events.VoltageInputVoltageChangeEventArgs e)
        {
            VoltageInput vi = (VoltageInput)sender;
            Tuple<VoltageInput, bool, SensorType> t = voltageInputs.Where(x => x.Item1.DeviceSerialNumber == vi.DeviceSerialNumber &&
                                                              x.Item1.HubPort == vi.HubPort).FirstOrDefault();
            if (e.Voltage > 0.01 && !t.Item2)
            {
                VoltageInput_Attached(t);
            }
            else if (e.Voltage <= 0.01 && t.Item2)
            {
                VoltageInput_Detached(t);
            }
        }

        private void VoltageInput_Attached(Tuple<VoltageInput, bool, SensorType> t)
        {
            Tuple<VoltageInput, bool, SensorType> newTuple = Tuple.Create(t.Item1, true, t.Item3);

            voltageInputs.Remove(t);
            voltageInputs.Add(newTuple);

            // start data collection
            Console.WriteLine($"{t.Item3} on {t.Item1.HubPort} on hub {t.Item1.DeviceSerialNumber} was attached");
            // update display
        }

        private void VoltageInput_Detached(Tuple<VoltageInput, bool, SensorType> t)
        {
            Tuple<VoltageInput, bool, SensorType> newTuple = Tuple.Create(t.Item1, true, t.Item3);

            voltageInputs.Remove(t);
            voltageInputs.Add(newTuple);

            // start data collection
            Console.WriteLine($"{t.Item3} on {t.Item1.HubPort} on hub {t.Item1.DeviceSerialNumber} was detached");
            // update display
        }

        private void Hub_Attach(object sender, Phidget22.Events.AttachEventArgs e)
        {
            Console.WriteLine($"Hub {((Hub)sender).DeviceSerialNumber} was attached");
            // update display
        }

        private void Hub_Detach(object sender, Phidget22.Events.DetachEventArgs e)
        {
            Console.WriteLine($"Hub {((Hub)sender).DeviceSerialNumber} was detached");
            // update display
        }

        private void Channel_Attach(object sender, Phidget22.Events.AttachEventArgs e)
        {
            if (((Phidget)sender).ChannelClassName != "PhidgetVoltageInput")
            {
                // update display
                // start data collection
                Console.WriteLine($"{((Phidget)sender).ChannelClassName} on {((Phidget)sender).HubPort} on hub {((Phidget)sender).DeviceSerialNumber} was attached");
            }
        }

        private void Channel_Detach(object sender, Phidget22.Events.DetachEventArgs e)
        {
            Console.WriteLine($"{((Phidget)sender).ChannelClassName} on {((Phidget)sender).HubPort} on hub {((Phidget)sender).DeviceSerialNumber} was detached");
            // update display
        }
    }
}
