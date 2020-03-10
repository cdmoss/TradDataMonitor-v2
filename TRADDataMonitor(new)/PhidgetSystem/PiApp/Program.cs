using Phidget22;
using System;
using System.Collections.Generic;
using SystemCore;

namespace PiApp
{
    public class Program
    {
        

        static void Main(string[] args)
        {
            //// Dictionary<>
            //
            ////Phidget ts = new TemperatureSensor();
            ////Phidget ls = new LightSensor();
            ////
            ////ls.Attach += Phidget_Attach;
            ////ls.Detach += Test_Detach;
            ////ts.Attach += Phidget_Attach;
            ////ts.Detach += Test_Detach;
            ////
            ////Hub hub = new Hub();
            ////hub.Attach += Hub_Attach;
            ////hub.Detach += Test_Detach;
            ////
            ////hub.Open();
            ////ts.Open();
            ////ls.Open();
            ////
            ////phidgets.Add(ls);
            //
            //VoltageInput vi = new VoltageInput();
            //vi.Attach += Test_Attach;
            //vi.Open();
            //
            //Console.WriteLine("Waiting for attachment");
            //
            //Console.ReadKey();

            Trailer t = ConfigurationManager.GetConfig();

            SensorManager sm = new SensorManager(t);

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
    }
}
