using System;
using System.Collections.Generic;
using System.Linq;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.Services;
using NAudio.Midi;

namespace LightPi.Midi2OrchestratorBridge.ViewModels.Mappings
{
    public class MappingEditorViewModel : BaseViewModel, IDialogViewModel
    {
        private readonly IMidiService _midiService;

        public MappingEditorViewModel(IMidiService midiService, ISettingsService settingsService)
        {
            if (midiService == null) throw new ArgumentNullException(nameof(midiService));

            _midiService = midiService;
            _midiService.NoteEventReceived += FillFromPressedNote;

            for (var i = 0; i < 12; i++)
            {
                Channels.Add(new SelectableViewModel<int>(i));
            }
            
            for (var i = 0; i < 8; i++)
            {
                Octaves.Add(new SelectableViewModel<int>(i));
            }

            Outputs.AddRange(settingsService.OutputViewModels.Select(o => new SelectableViewModel<OutputViewModel>(o)));

            Notes.Add(new SelectableViewModel<string>("C"));
            Notes.Add(new SelectableViewModel<string>("C#"));
            Notes.Add(new SelectableViewModel<string>("D"));
            Notes.Add(new SelectableViewModel<string>("D#"));
            Notes.Add(new SelectableViewModel<string>("E"));
            Notes.Add(new SelectableViewModel<string>("F"));
            Notes.Add(new SelectableViewModel<string>("F#"));
            Notes.Add(new SelectableViewModel<string>("G"));
            Notes.Add(new SelectableViewModel<string>("G#"));
            Notes.Add(new SelectableViewModel<string>("A"));
            Notes.Add(new SelectableViewModel<string>("A#/B"));
            Notes.Add(new SelectableViewModel<string>("H"));
        }

        public List<SelectableViewModel<int>> Channels { get; } = new List<SelectableViewModel<int>>();
        
        public List<SelectableViewModel<string>> Notes { get; } = new List<SelectableViewModel<string>>();
        
        public List<SelectableViewModel<int>> Octaves { get; } = new List<SelectableViewModel<int>>();

        public List<SelectableViewModel<OutputViewModel>> Outputs { get; } = new List<SelectableViewModel<OutputViewModel>>();
        
        public string Comment { get; set; }

        public void Load(Mapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Reset();

            Channels.SelectMatching(c => c.Model == mapping.Channel);
            Notes.SelectMatching(c => c.Model == mapping.Note);
            Octaves.SelectMatching(o => o.Model == mapping.Octave);
            Outputs.SelectMatching(o => mapping.Outputs.Contains(o.Model.Output.Id));
            Comment = mapping.Comment;
        }

        public void Update(Mapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            mapping.Channel = Channels.First(c => c.IsSelected).Model;
            mapping.Note = Notes.First(c => c.IsSelected).Model;
            mapping.Octave = Octaves.First(c => c.IsSelected).Model;
            mapping.Outputs = Outputs.Where(o => o.IsSelected).Select(o => o.Model.Output.Id).ToList();
            mapping.Comment = Comment;
        }

        public void Close(DialogResult dialogResult)
        {
            _midiService.NoteEventReceived -= FillFromPressedNote;
        }

        public void Reset()
        {
            Channels.ForEach(c => c.Deselect());
            Notes.ForEach(c => c.Deselect());
            Octaves.ForEach(c => c.Deselect());
            Outputs.ForEach(c => c.Deselect());
            
            Comment = string.Empty;
        }

        private void FillFromPressedNote(object sender, NoteEventReceivedEventArgs e)
        {
            if (e.Command != MidiCommandCode.NoteOn)
            {
                return;
            } 

            Channels.SelectMatching(c => c.Model == e.Channel);
            Notes.SelectMatching(c => c.Model == e.Note);
            Octaves.SelectMatching(o => o.Model == e.Octave);
        }
    }
}
