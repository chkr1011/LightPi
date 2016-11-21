using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.ViewModels.Mappings;
using LightPi.Protocol;
using Newtonsoft.Json.Linq;

namespace LightPi.Midi2OrchestratorBridge.Services
{
    public class SettingsService : ISettingsService
    {
        public Settings Settings { get; private set; } = new Settings();

        public List<MappingViewModel> MappingViewModels { get; } = new List<MappingViewModel>();

        public List<OutputViewModel> OutputViewModels { get; } = new List<OutputViewModel>();

        public void Load()
        {
            var filename = GenerateFilename();
            if (!File.Exists(filename))
            {
                return;
            }

            var fileContent = File.ReadAllText(filename);
            var json = JObject.Parse(fileContent);
            var settings = json.ToObject<Settings>();

            if (settings.Mappings == null)
            {
                settings.Mappings = new List<Mapping>();
            }
            else
            {
                foreach (var mapping in settings.Mappings)
                {
                    if (mapping.Outputs == null)
                    {
                        mapping.Outputs = new List<int>();
                    }
                }
            }

            Settings = settings;
            GenerateViewModels();
        }

        public void Save()
        {
            var json = JObject.FromObject(Settings);
            File.WriteAllText(GenerateFilename(), json.ToString());
        }

        public void ImportMappingViewModels(ICollection<MappingViewModel> mappings)
        {
            MappingViewModels.Clear();
            MappingViewModels.AddRange(mappings);

            Settings.Mappings.Clear();
            Settings.Mappings.AddRange(mappings.Select(m => m.Mapping));
        }

        private string GenerateFilename()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LightPi.Midi2OrchestratorBridgeSettings.json");
        }

        private void GenerateViewModels()
        {
            OutputViewModels.Clear();
            for (var i = 0; i < LightPiProtocol.OutputsCount; i++)
            {
                Output output;
                if (!Settings.Outputs.TryGetValue(i, out output))
                {
                    output = new Output {Id = i};
                }

                OutputViewModels.Add(new OutputViewModel(output));
            }

            MappingViewModels.Clear();
            foreach (var mapping in Settings.Mappings)
            {
                MappingViewModels.Add(new MappingViewModel(mapping));       
            }
        }
    }
}
