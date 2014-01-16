using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using ProcessProxifier.Models;

namespace ProcessProxifier.Core
{
    public static class SettingsSerializer
    {
        public static void SaveSettings(ProxifierSettings data, string configFilePath)
        {
            using (var fileStream = new FileStream(configFilePath, FileMode.Create))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    var ns = new XmlSerializerNamespaces(); ns.Add("", "");
                    var xmlSerializer = new XmlSerializer(typeof(ProxifierSettings));
                    xmlSerializer.Serialize(streamWriter, data, ns);                    
                }
            }
        }

        public static ProxifierSettings LoadSettings(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                SaveSettings(new ProxifierSettings(), configFilePath);
            }

            var xmlSerializer = new XmlSerializer(typeof(ProxifierSettings));
            var ctx = XDocument.Load(configFilePath);
            return (ProxifierSettings)xmlSerializer.Deserialize(ctx.Root.CreateReader());
        }
    }
}