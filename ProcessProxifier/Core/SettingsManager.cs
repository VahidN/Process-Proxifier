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
            get { return Path.Combine(Application.StartupPath, "settings.json"); }
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

        public static void SaveSettings(ProxifierSettings guiModelData, ProxifierSettings settings)
        {
            if (guiModelData.RunOnStartup)
                RunOnWindowsStartup.Do();
            else
                RunOnWindowsStartup.Undo();

            addNewActiveItems(guiModelData, settings);
            SettingsSerializer.SaveSettings(guiModelData, SettingsPath);
        }

        private static void addNewActiveItems(ProxifierSettings guiModelData, ProxifierSettings settings)
        {
            foreach (var process in guiModelData.ProcessesList)
            {
                if (!process.IsEnabled && string.IsNullOrWhiteSpace(process.ServerInfo.ServerIP) &&
                    process.ServerInfo.ServerPort == 0)
                {
                    continue;
                }

                if(!settings.ActiveProcessesList.Contains(process))
                {
                    settings.ActiveProcessesList.Add(process);
                }

                if (!guiModelData.ActiveProcessesList.Contains(process))
                {
                    guiModelData.ActiveProcessesList.Add(process);
                }
            }
        }
    }
}