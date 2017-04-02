using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using ProcessProxifier.Models;
using ProcessProxifier.Utils;

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
            var result = (ProxifierSettings)xmlSerializer.Deserialize(ctx.Root.CreateReader());
            result.ProcessesList = new AsyncObservableCollection<Process>();
            result.RoutedConnectionsList = new AsyncObservableCollection<RoutedConnection>();
            return result;
        }
    }
}