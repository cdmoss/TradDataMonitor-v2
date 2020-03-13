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
            Trailer t = ConfigurationManager.GetConfig();

            SensorManager sm = new SensorManager(t);

            Console.ReadKey();
        }
    }
}
