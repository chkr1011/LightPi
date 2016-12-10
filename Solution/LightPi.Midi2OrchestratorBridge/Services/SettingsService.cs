using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.ViewModels.Mappings;
using LightPi.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LightPi.Midi2OrchestratorBridge.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _profilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Profiles");
        private readonly string _settingsFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LightPi.Midi2OrchestratorBridgeSettings.json");

        public Settings Settings { get; private set; } = new Settings();

        public List<OutputViewModel> OutputViewModels { get; } = new List<OutputViewModel>();

        public List<ProfileViewModel> ProfileViewModels { get; } = new List<ProfileViewModel>();

        public void Load()
        {
            CreateProfilesDirectory();

            LoadSettings();
            LoadProfiles();
        }

        private void LoadSettings()
        {
            if (!File.Exists(_settingsFilename))
            {
                return;
            }

            var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_settingsFilename));

            if (settings.Outputs == null)
            {
                settings.Outputs = new Dictionary<int, Output>();
            }

            Settings = settings;
            GenerateViewModels();
        }

        private void LoadProfiles()
        {
            ProfileViewModels.Clear();
            foreach (var file in Directory.GetFiles(_profilesDirectory, "*.json"))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var profileViewModel = new ProfileViewModel
                {
                    Name = name
                };

                var mappings = JsonConvert.DeserializeObject<List<Mapping>>(File.ReadAllText(file));
                foreach (var mapping in mappings)
                {
                    var mappingViewModel = new MappingViewModel
                    {
                        Channel = mapping.Channel,
                        Note = mapping.Note,
                        Octave = mapping.Octave,
                        Comment = mapping.Comment
                    };

                    foreach (var output in mapping.Outputs)
                    {
                        mappingViewModel.Outputs.Add(OutputViewModels.FirstOrDefault(o => o.Output.Id == output));
                    }

                    profileViewModel.Mappings.Add(mappingViewModel);
                }

                ProfileViewModels.Add(profileViewModel);
            }
        }

        private void CreateProfilesDirectory()
        {
            if (Directory.Exists(_profilesDirectory))
            {
                return;
            }

            Directory.CreateDirectory(_profilesDirectory);
        }

        public void SaveSettings()
        {
            var json = JObject.FromObject(Settings);
            File.WriteAllText(_settingsFilename, json.ToString());
        }

        public void SaveProfile(ProfileViewModel profile)
        {
            var filename = Path.Combine(_profilesDirectory, profile.Name + ".json");
            var mappings = profile.Mappings.Select(m => new Mapping
            {
                Channel = m.Channel,
                Note = m.Note,
                Octave = m.Octave,
                Comment = m.Comment,
                Outputs = m.Outputs.Select(o => o.Output.Id).ToList()
            });

            File.WriteAllText(filename, JsonConvert.SerializeObject(mappings));
        }

        private void GenerateViewModels()
        {
            OutputViewModels.Clear();
            for (var i = 0; i < LightPiProtocol.OutputsCount; i++)
            {
                Output output;
                if (!Settings.Outputs.TryGetValue(i, out output))
                {
                    output = new Output { Id = i };
                }

                OutputViewModels.Add(new OutputViewModel(output));
            }
        }
    }
}
