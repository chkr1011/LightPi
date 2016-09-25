using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace LightPi.OrchestratorEmulator.Settings
{
    public class SettingsService
    {
        private readonly string _filename;

        public SettingsService(string filename)
        {
            if (filename == null) throw new ArgumentNullException(nameof(filename));

            _filename = filename;
        }

        public Settings Settings { get; private set; } = new Settings();

        public void Load()
        {
            if (!File.Exists(_filename))
            {
                Settings.OutputDefinitions.Add(new OutputDefinition(1, 1));

                Save();
                return;
            }

            var serializer = new DataContractSerializer(typeof(Settings));
            using (var stream = File.OpenRead(_filename))
            {
                Settings = (Settings)serializer.ReadObject(stream);
            }
        }

        public void Save()
        {
            var serializer = new DataContractSerializer(typeof(Settings));
            using (var stream = File.OpenWrite(_filename))
            using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                serializer.WriteObject(writer, Settings);
            }
        }
    }
}
