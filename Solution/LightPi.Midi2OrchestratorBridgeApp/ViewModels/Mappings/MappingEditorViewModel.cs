using System;
using System.Linq;
using System.Text.RegularExpressions;
using LightPi.Midi2OrchestratorBridgeApp.Models;
using LightPi.Midi2OrchestratorBridgeApp.Services;
using NAudio.Midi;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels.Mappings
{
    public class MappingEditorViewModel : BaseViewModel, IDialogViewModel
    {
        private readonly IMidiService _midiService;
        private readonly ILogService _logService;
        private string _note;
        private int _octave;
        private MidiChannel _channel;

        public MappingEditorViewModel(IMidiService midiService, ILogService logService)
        {
            if (midiService == null) throw new ArgumentNullException(nameof(midiService));
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            _midiService = midiService;
            _logService = logService;
            _midiService.MidiMessageReceived += FillFromPressedNote;

            Channels = Enum.GetValues(typeof(MidiChannel)).Cast<MidiChannel>().ToArray();
            
            Octaves = new int[8];
            for (int i = 0; i < Octaves.Length; i++)
            {
                Octaves[i] = i;
            }

            Outputs = new int[48];
            for (int i = 0; i < Outputs.Length; i++)
            {
                Outputs[i] = i;
            }

            Channel = Channels.FirstOrDefault();
            Note = Notes.FirstOrDefault();
            Octave = Octaves.FirstOrDefault();
            Output = Outputs.FirstOrDefault();
        }

        public MidiChannel[] Channels { get; }

        public MidiChannel Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                OnPropertyChanged();
            }
        }

        public string[] Notes { get; } = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A# ,(B)", "H" };

        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                OnPropertyChanged();
            }
        }

        public int[] Octaves { get; }

        public int Octave
        {
            get { return _octave; }
            set
            {
                _octave = value;
                OnPropertyChanged();
            }
        }

        public string CompleteNote => Note + Octave;

        public int[] Outputs { get; }

        public int Output { get; set; }

        public string Comment { get; set; }

        public void Load(Mapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Channel = mapping.Channel;
            FillNoteFromString(mapping.Note);
            Output = mapping.Output;
            Comment = mapping.Comment;
        }

        public void Close(DialogResult dialogResult)
        {
            _midiService.MidiMessageReceived -= FillFromPressedNote;
        }

        public void Reset()
        {
            Channel = MidiChannel.Channel0;
            Octave = 0;
            Note = Notes.First();
            Output = Outputs.First();
            Comment = string.Empty;
        }

        private void FillFromPressedNote(object sender, MidiMessageReceivedEventArgs e)
        {
            if (e.Note.CommandCode != MidiCommandCode.NoteOn)
            {
                return;
            }

            Channel = (MidiChannel) e.Note.Channel - 1;
            FillNoteFromString(e.Note.NoteName);
        }

        private void FillNoteFromString(string note)
        {
            if (string.IsNullOrEmpty(note))
            {
                return;
            }

            var noteMatch = Regex.Match(note, "(?<note>[A-Z]{1,1}#{0,1})(?<octave>[0-9]{1})");

            try
            {
                Note = noteMatch.Groups["note"].Value;
                Octave = int.Parse(noteMatch.Groups["octave"].Value);
            }
            catch (Exception)
            {
                _logService.Warning($"Unable to parse note information from 'note'");
            }
            
        }
    }
}
