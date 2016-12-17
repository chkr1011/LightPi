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
                Channels.Add(new SelectableViewModel<int>(i) { IsSelected = i == 2 });
            }

            for (var i = 0; i < 9; i++)
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
            Notes.Add(new SelectableViewModel<string>("A#"));
            Notes.Add(new SelectableViewModel<string>("B"));

            RouteCommand(MappingsCommand.Import, ImportOutputsFromCurrentState);
            RouteCommand(MappingsCommand.Reset, ResetOutputs);

            ImportOutputsFromCurrentState();
        }

        public List<SelectableViewModel<int>> Channels { get; } = new List<SelectableViewModel<int>>();

        public List<SelectableViewModel<string>> Notes { get; } = new List<SelectableViewModel<string>>();

        public List<SelectableViewModel<int>> Octaves { get; } = new List<SelectableViewModel<int>>();

        public List<SelectableViewModel<OutputViewModel>> Outputs { get; } = new List<SelectableViewModel<OutputViewModel>>();

        public string Comment { get; set; }

        public void Load(MappingViewModel mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Channels.SelectMatching(c => c.Model == mapping.Channel);
            Notes.SelectMatching(c => c.Model == mapping.Note);
            Octaves.SelectMatching(o => o.Model == mapping.Octave);
            Outputs.SelectMatching(o1 => mapping.Outputs.Any(o2 => o1.Model.Output.Id == o2.Output.Id));
            Comment = mapping.Comment;
        }

        public void FillNote(NoteEventReceivedEventArgs eventArgs)
        {
            if (eventArgs?.Command != MidiCommandCode.NoteOn)
            {
                return;
            }

            Channels.SelectMatching(c => c.Model == eventArgs.Channel);
            Notes.SelectMatching(c => c.Model == eventArgs.Note);
            Octaves.SelectMatching(o => o.Model == eventArgs.Octave);
        }

        public void Update(MappingViewModel mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            mapping.Channel = Channels.First(c => c.IsSelected).Model;
            mapping.Note = Notes.First(c => c.IsSelected).Model;
            mapping.Octave = Octaves.First(c => c.IsSelected).Model;
            mapping.Comment = Comment;

            mapping.Outputs.Clear();
            foreach (var output in Outputs)
            {
                if (!output.IsSelected)
                {
                    continue;
                }

                mapping.Outputs.Add(output.Model);
            }
        }

        public void Close(DialogResult dialogResult)
        {
            _midiService.NoteEventReceived -= FillFromPressedNote;
        }

        private void FillFromPressedNote(object sender, NoteEventReceivedEventArgs e)
        {
            FillNote(e);
        }

        private void ImportOutputsFromCurrentState()
        {
            foreach (var output in Outputs)
            {
                output.IsSelected = output.Model.IsActive;
            }
        }

        private void ResetOutputs()
        {
            foreach (var output in Outputs)
            {
                output.IsSelected = false;
            }
        }
    }
}
