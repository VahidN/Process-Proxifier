using System.IO;
using Newtonsoft.Json;
using ProcessProxifier.Models;

namespace ProcessProxifier.Core
{
    public static class SettingsSerializer
    {
        public static void SaveSettings(ProxifierSettings data, string configFilePath)
        {
            File.WriteAllText(configFilePath, JsonConvert.SerializeObject(data, new JsonSerializerSettings { Formatting = Formatting.Indented  }));
        }

        public static ProxifierSettings LoadSettings(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                SaveSettings(new ProxifierSettings(), configFilePath);
            }

            return JsonConvert.DeserializeObject<ProxifierSettings>(File.ReadAllText(configFilePath));
        }
    }
}