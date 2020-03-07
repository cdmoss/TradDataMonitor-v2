using Phidget22;
using System;
using System.Collections.Generic;

namespace PiApp
{
    public class Program
    {
        static List<Phidget> phidgets = new List<Phidget>();

        static void Main(string[] args)
        {
            // Dictionary<>

            //Phidget ts = new TemperatureSensor();
            //Phidget ls = new LightSensor();
            //
            //ls.Attach += Phidget_Attach;
            //ls.Detach += Test_Detach;
            //ts.Attach += Phidget_Attach;
            //ts.Detach += Test_Detach;
            //
            //Hub hub = new Hub();
            //hub.Attach += Hub_Attach;
            //hub.Detach += Test_Detach;
            //
            //hub.Open();
            //ts.Open();
            //ls.Open();
            //
            //phidgets.Add(ls);

            VoltageInput vi = new VoltageInput();
            vi.Attach += Test_Attach;
            vi.Open();

            Console.WriteLine("Waiting for attachment");

            Console.ReadKey();
        }

        // handler for voc attach
        private static void VOC_Attach()
        {

        }

        // handler for voltage attach
        private static void Voltage_Attach()
        {
            // check if this sensor's hub port is associated with oxygen or moisture
            if (true)
            {
                // oxygen
            }
            else
            {
                // moisture
            }
        }

        private static void Phidget_Attach(object sender, Phidget22.Events.AttachEventArgs e)
        {
            Console.WriteLine("A new sensor has been attached");
            Console.WriteLine(phidgets.Count.ToString());
            
            if (sender is HumiditySensor)
            {
                // read data from humidity sensor and temperature sensor
                Console.WriteLine("A Humidity/Air Temperature sensor has been attached");
            }
            if (sender is TemperatureSensor)
            {
                // check if this sensor's hub port is the same as humidity sensor
                if (true)
                {
                    // if it is, read data as air temp data
                }
                else
                {
                    // read as soil temp data
                }
            }
        }

        private static void Test_Attach(object sender, Phidget22.Events.AttachEventArgs e)
        {
            Console.WriteLine($"{((VoltageInput)sender).HubPort})");
            Console.WriteLine($"{((VoltageInput)sender).DeviceSerialNumber})");
        }

        private static void Test_Detach(object sender, Phidget22.Events.DetachEventArgs e)
        {
            // detached = true;
            if (sender is LightSensor)
            {
                ((LightSensor)sender).Close();
                phidgets.Remove((LightSensor)sender);
            }
            Console.WriteLine("Sensor has been disconnected");
            Console.WriteLine(phidgets.Count.ToString());
        }
    }
}
