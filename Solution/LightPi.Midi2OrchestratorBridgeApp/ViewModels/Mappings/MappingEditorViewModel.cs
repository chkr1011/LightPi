using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LightPi.Midi2OrchestratorBridgeApp.Models;
using LightPi.Midi2OrchestratorBridgeApp.Services;
using LightPi.Protocol;
using NAudio.Midi;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels.Mappings
{
    public class MappingEditorViewModel : BaseViewModel, IDialogViewModel
    {
        private readonly IMidiService _midiService;
        private readonly ILogService _logService;
        private string _note;
        private int _octave;
        private int _channel;

        public MappingEditorViewModel(IMidiService midiService, ILogService logService)
        {
            if (midiService == null) throw new ArgumentNullException(nameof(midiService));
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            _midiService = midiService;
            _logService = logService;
            _midiService.MidiMessageReceived += FillFromPressedNote;

            for (var i = 0; i < 12; i++)
            {
                Channels.Add(i);
            }
            
            for (var i = 0; i < 8; i++)
            {
                Octaves.Add(i);
            }

            for (var i = 0; i < LightPiProtocol.OutputsCount; i++)
            {
                Outputs.Add(i);
            }

            Channel = Channels.FirstOrDefault();
            Note = Notes.FirstOrDefault();
            Octave = Octaves.FirstOrDefault();
            Output = Outputs.FirstOrDefault();
        }

        public IList<int> Channels { get; } = new List<int>();

        public int Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                OnPropertyChanged();
            }
        }

        public IList<string> Notes { get; } = new List<string> { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A# ,(B)", "H" };

        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                OnPropertyChanged();
            }
        }

        public IList<int> Octaves { get; } = new List<int>();

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

        public IList<int> Outputs { get; } = new List<int>();

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
            Channel = 0;
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

            Channel = e.Note.Channel;
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
                _logService.Warning("Unable to parse note information from 'note'");
            }
        }
    }
}
