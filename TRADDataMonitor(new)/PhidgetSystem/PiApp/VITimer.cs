using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace PiApp
{
    public class VITimer
    {
        public Timer Timer { get; set; }
        public int HubPort { get; set; }

        public VITimer(int port)
        {
            this.HubPort = port;
            Timer = new Timer();
        }
    }
}
