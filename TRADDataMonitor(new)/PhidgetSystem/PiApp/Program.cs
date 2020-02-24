using Phidget22;
using System;

namespace PiApp
{
    public class Program
    {
        static bool detached = false;
        static void Main(string[] args)
        {

            TemperatureSensor test = new TemperatureSensor();
            test.HubPort = 2;
            test.DeviceSerialNumber = 559023;
            test.Attach += Test_Attach;
            test.Detach += Test_Detach;

            test.Open();

            Console.WriteLine("Waiting for attachment");

            Console.ReadKey();
        }

        private static void Test_Detach(object sender, Phidget22.Events.DetachEventArgs e)
        {
            detached = true;
            Console.WriteLine("Sensor has been disconnected");
        }

        private static void Test_Attach(object sender, Phidget22.Events.AttachEventArgs e)
        {
            Console.WriteLine("Sensor has been connected");
        }
    }
}
