using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using LightPi.Midi2OrchestratorBridgeApp.Models;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public class SettingsService   
    {
        public Settings Settings { get; private set; } = new Settings();

        public void Load()
        {
            var serializer = new DataContractSerializer(typeof(Settings));

            var filename = GenerateFilename();
            if (!File.Exists(filename))
            {
                return;
            }

            using (var fileStream = File.OpenRead(filename))
            {
                var settings = (Settings)serializer.ReadObject(fileStream);
                if (settings.Mappings == null)
                {
                    settings.Mappings = new List<Mapping>();
                }

                Settings = settings;
            }
        }

        public void Save()
        {
            var serializer = new DataContractSerializer(typeof(Settings));
            using (var fileStream = File.Create(GenerateFilename()))
            using (var xmlWriter = new XmlTextWriter(fileStream, Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                
                serializer.WriteObject(xmlWriter, Settings);
            }
        }

        private string GenerateFilename()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LightPi.Midi2OrchestratorBridgeSettings.xml");
        }
    }
}
