using System.IO;
using System.Windows.Forms;
using ProcessProxifier.Models;
using ProcessProxifier.Utils;

namespace ProcessProxifier.Core
{
    public static class SettingsManager
    {
        public static string SettingsPath
        {
            get { return Path.Combine(Application.StartupPath, "settings.xml"); }
        }

        public static ProxifierSettings LoadSettings(ProxifierSettings guiModelData)
        {
            var settings = SettingsSerializer.LoadSettings(SettingsPath);
            guiModelData.ProxifierPort = settings.ProxifierPort;
            guiModelData.DefaultServerInfo.ServerIP = settings.DefaultServerInfo.ServerIP;
            guiModelData.DefaultServerInfo.ServerPort = settings.DefaultServerInfo.ServerPort;
            guiModelData.DefaultServerInfo.ServerType = settings.DefaultServerInfo.ServerType;
            guiModelData.RunOnStartup = settings.RunOnStartup;
            guiModelData.AreAllChecked = settings.AreAllChecked;
            return settings;
        }

        public static void SaveSettings(ProxifierSettings guiModelData)
        {
            if (guiModelData.RunOnStartup)
                RunOnWindowsStartup.Do();
            else
                RunOnWindowsStartup.Undo();

            SettingsSerializer.SaveSettings(guiModelData, SettingsPath);
        }
    }
}