using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace LightPi.OrchestratorEmulator
{
    public class SettingsReader
    {
        private readonly string _filename;
        private XDocument _xml;

        public SettingsReader(string filename)
        {
            if (filename == null) throw new ArgumentNullException(nameof(filename));

            _filename = filename;
        }

        public void Load()
        {
            _xml = XDocument.Load(_filename);
        }

        public string GetBackgroundSpriteFilename()
        {
            ThrowIfNotLoaded();

            return _xml.Root.Element("Background").Attribute("Sprite").Value;
        }

        public List<XElement> GetOutputs()
        {
            ThrowIfNotLoaded();

            return _xml.Root.Element("Outputs").Elements("Output").ToList();
        }

        public IPAddress GetOrchestratorAddress()
        {
            return IPAddress.Parse(_xml.Root.Element("OrchestratorAddress").Value);
        }

        private void ThrowIfNotLoaded()
        {
            if (_xml == null)
            {
                throw new Exception("XML is not loaded.");
            }
        }
    }
}
