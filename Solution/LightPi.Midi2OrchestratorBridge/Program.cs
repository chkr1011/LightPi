using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightPi.Orchestrator;
using LightPi.Protocol;

namespace LightPi.Midi2OrchestratorBridge
{
    class Program
    {
        static void Main(string[] args)
        {
            var orchestrator = new OrchestratorClient(IPAddress.Parse("192.168.1.107"));

            byte[] frame = new byte[6];

            bool state = true;
            int i = 0;

            while (true)
            {
                frame.SetBit(i, state);
                i++;

                if (i > 15)
                {
                    i = 0;
                    state = !state;
                }

                orchestrator.SendFrame(frame);
                Thread.Sleep(50);

                ////Console.WriteLine("Sent " + frame[0]);
            }
        }
    }
}
