using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using LightPi.Midi2OrchestratorBridgeApp.Models;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly DataContractSerializer _serializer = new DataContractSerializer(typeof(Settings), new[] { typeof(Output), typeof(OutputGroup) });
        public Settings Settings { get; private set; } = new Settings();

        public void Load()
        {
            var filename = GenerateFilename();
            if (!File.Exists(filename))
            {
                return;
            }

            using (var fileStream = File.OpenRead(filename))
            {
                var settings = (Settings)_serializer.ReadObject(fileStream);
                if (settings.Mappings == null)
                {
                    settings.Mappings = new List<Mapping>();
                }

                if (settings.Outputs == null)
                {
                    settings.Outputs = new List<IOutput>();
                }

                Settings = settings;
            }
        }

        public void Save()
        {
            using (var fileStream = File.Create(GenerateFilename()))
            using (var xmlWriter = new XmlTextWriter(fileStream, Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;

                _serializer.WriteObject(xmlWriter, Settings);
            }
        }

        private string GenerateFilename()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LightPi.Midi2OrchestratorBridgeSettings.xml");
        }
    }
}
