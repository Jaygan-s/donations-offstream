using SocketIOClient.ConnectInterval;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamerClient
{
    class ConnectInverval : IConnectInterval
    {
        private double delay;

        public ConnectInverval()
        {
            delay = 3000.0;
        }

        public int GetDelay()
        {
            return (int)delay;
        }

        public double NextDealy()
        {
            return delay += 3000.0;
        }
    }
}
