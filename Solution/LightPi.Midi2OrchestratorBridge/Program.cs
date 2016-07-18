using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using LightPi.Orchestrator;
using LightPi.Protocol;
using NAudio.Midi;

namespace LightPi.Midi2OrchestratorBridge
{
    internal static class Program
    {
        private readonly static Dictionary<string, int> _mappings = new Dictionary<string, int>();

        private static MidiIn _midiIn;
        private static OrchestratorClient _orchestratorClient;

        public static void Main(string[] args)
        {
            WriteBanner();

            XDocument settings = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.xml"));

            InitializeOrchestratorClient(settings);
            LoadMappings(settings);
            InitializeMidiInput(settings);

            WriteOutput(ConsoleColor.Gray, "Press any key to exit.");
            Console.ReadLine();

            bool state = true;
            while (true)
            {
                state = !state;
                _orchestratorClient.Frame.SetBit(20, state);

                _orchestratorClient.SendFrame();
                Thread.Sleep(100);


                ////frame.SetBit(i, state);
                ////i++;

                ////if (i > 20)
                ////{
                ////    i = 0;
                ////    state = !state;
                ////}

                ////orchestrator.SendFrame(frame);
                ////Thread.Sleep(50);

                ////Console.WriteLine("Sent " + frame[0]);
            }
        }

        private static void WriteBanner()
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine("|                            |");
            Console.WriteLine("| MIDI 2 Orchestrator Bridge |");
            Console.WriteLine("|                            |");
            Console.WriteLine("------------------------------");
        }

        private static void InitializeMidiInput(XDocument settings)
        {
            string midiDevideFromSettings = settings.Root.Element("MidiIn").Value;
            
            WriteOutput(ConsoleColor.Magenta, "MIDI devices:");

            int deviceId = 0;
            int m = MidiIn.NumberOfDevices;
            for (int i = 0; i < m; i++)
            {
                var devicInfo = MidiIn.DeviceInfo(i);
                WriteOutput(ConsoleColor.Magenta, i + " = " + devicInfo.ProductName);

                if (devicInfo.ProductName == midiDevideFromSettings)
                {
                    deviceId = i;
                }
            }
            
            _midiIn = new MidiIn(deviceId);
            _midiIn.MessageReceived += ProcessMidiMessage;
            _midiIn.ErrorReceived += ProcessMidiError;
            _midiIn.Start();
        }

        private static void InitializeOrchestratorClient(XDocument settings)
        {
            var ipAddress = IPAddress.Parse(settings.Root.Element("OrchestratorIPAddress").Value);
            
            WriteOutput(ConsoleColor.Green, $"Using orchestrator at IP address [{ipAddress}]");
            _orchestratorClient = new OrchestratorClient(ipAddress);
            _orchestratorClient.SendFrame();
        }

        private static void LoadMappings(XDocument settings)
        {
            var mappings = settings.Root.Element("Mappings").Elements("Mapping");
            foreach (var mapping in mappings)
            {
                var note = mapping.Attribute("Note").Value;
                var bitText = mapping.Attribute("Bit").Value;
                var comment = mapping.Attribute("Comment").Value;

                var bit = int.Parse(bitText);
                _mappings[note] = bit;

                WriteOutput(ConsoleColor.Green, $"Mapped [{note}] to bit [{bitText}] ({comment})");
            }
        }

        private static void ProcessMidiMessage(object sender, MidiInMessageEventArgs e)
        {
            var noteEvent = e.MidiEvent as NoteEvent;
            if (noteEvent == null)
            {
                WriteOutput(ConsoleColor.Red, "Unsupported event: " + e.MidiEvent);
                return;
            }

            WriteOutput(ConsoleColor.Gray, e.MidiEvent.CommandCode + " " + noteEvent.NoteName);
            
            bool newState = noteEvent.CommandCode == MidiCommandCode.NoteOn;

            int bit;
            if (_mappings.TryGetValue(noteEvent.NoteName, out bit))
            {
                _orchestratorClient.Frame.SetBit(bit, newState);

                var color = newState ? ConsoleColor.Green : ConsoleColor.Yellow;
                WriteOutput(color, "Set bit [" + bit + "] to [" + newState + "]");

                _orchestratorClient.SendFrame();

                WriteOutput(ConsoleColor.Gray, "Sent [" + BitConverter.ToString(_orchestratorClient.Frame) + "] to orchestrator");
            }           
        }

        private static void ProcessMidiError(object sender, MidiInMessageEventArgs e)
        {
            WriteOutput(ConsoleColor.Red, "Error event: " + e.MidiEvent);
        }

        private static void WriteOutput(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
        }
    }
}
