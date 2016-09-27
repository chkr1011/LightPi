using System;
using System.Collections.Generic;
using System.Linq;
using LightPi.Midi2OrchestratorBridgeApp.Models;
using NAudio.Midi;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public class MidiService : IMidiService
    {
        private readonly ISettingsService _settingsService;
        private readonly ILogService _logService;
        private MidiIn _attachedMidiPort;

        public MidiService(ISettingsService settingsService, ILogService logService)
        {
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            _settingsService = settingsService;
            _logService = logService;
        }

        public event EventHandler<MidiMessageReceivedEventArgs> MidiMessageReceived; 

        public IList<MidiPort> GetMidiPorts()
        {
            var midiPorts = new List<MidiPort>();
            
            _logService.Verbose("Searching for MIDI input ports");
            int numberOfDevices = MidiIn.NumberOfDevices;
            _logService.Verbose($"Found {numberOfDevices} MIDI input ports");

            for (int i = 0; i < numberOfDevices; i++)
            {
                var midiPortInfo = MidiIn.DeviceInfo(i);

                var midiPort = new MidiPort(i, midiPortInfo.ProductName);
                midiPorts.Add(midiPort);
            }
            
            midiPorts.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));

            return midiPorts;
        }

        public void Initialize()
        {
            CleanupMidiPort();
            AttachMidiPort();
        }

        private void ProcessMidiMessage(object sender, MidiInMessageEventArgs e)
        {
            var noteEvent = e.MidiEvent as NoteEvent;
            if (noteEvent == null)
            {
                _logService.Warning($"Received unsupported MIDI event: {e.MidiEvent}");
                return;
            }

            _logService.Verbose($"Received MIDI event: Ch:{noteEvent.Channel} / N:{noteEvent.NoteName} ({noteEvent.NoteNumber}) / C:{noteEvent.CommandCode} / V:{noteEvent.Velocity}");

            MidiMessageReceived?.Invoke(this, new MidiMessageReceivedEventArgs(noteEvent));
        }

        private void LogMidiError(object sender, MidiInMessageEventArgs e)
        {
            _logService.Error($"MIDI error: {e.MidiEvent.GetAsShortMessage()}");
        }

        private void AttachMidiPort()
        {
            var midiPorts = GetMidiPorts();
            var selectedMidiPort = midiPorts.FirstOrDefault(p => p.Name.Equals(_settingsService.Settings.MidiIn));
            if (selectedMidiPort == null)
            {
                _logService.Warning("MIDI port not found.");
                return;
            }

            _attachedMidiPort = new MidiIn(selectedMidiPort.Id);
            _attachedMidiPort.MessageReceived += ProcessMidiMessage;
            _attachedMidiPort.ErrorReceived += LogMidiError;
            _attachedMidiPort.Start();

            _logService.Information("MIDI port is attached");
        }

        private void CleanupMidiPort()
        {
            if (_attachedMidiPort == null)
            {
                return;
            }

            _logService.Information("Disconnecting already attached MIDI port");
            _attachedMidiPort.Stop();
            _attachedMidiPort.Close();
            _attachedMidiPort.Dispose();

            _attachedMidiPort.MessageReceived -= ProcessMidiMessage;
            _attachedMidiPort.ErrorReceived -= LogMidiError;
        }
    }
}
