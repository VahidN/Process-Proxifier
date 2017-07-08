using System;
using System.Linq;
using ProcessProxifier.Models;
using ProcessProxifier.Utils;

namespace ProcessProxifier.Core
{
    public static class ProcessesListManager
    {
        public static AsyncObservableCollection<Process> UpdateProcesses(
            ProxifierSettings guiModelData,
            ProxifierSettings settings)
        {
            var systemProcessList = System.Diagnostics.Process.GetProcesses().OrderBy(x => x.ProcessName).ToList();
            var systemProcessIds = systemProcessList.Select(p => p.Id).ToList();
            var finishedProcesses = guiModelData.ProcessesList
                      .Where(process => !systemProcessIds.Contains(process.Pid))
                      .ToList();

            if (finishedProcesses.Any())
            {
                SettingsManager.SaveSettings(guiModelData, settings);
            }

            foreach (var process in finishedProcesses)
            {
                guiModelData.ProcessesList.Remove(process);
            }

            var guiProcessIds = guiModelData.ProcessesList.Select(process => process.Pid).ToList();
            var newSystemProcesses = systemProcessList.Where(process => !guiProcessIds.Contains(process.Id)).ToList();
            foreach (var systemProcess in newSystemProcesses)
            {
                var path = systemProcess.GetPath();
                if(string.IsNullOrWhiteSpace(path)) // TODO: improve
                {
                    continue;
                }

                var newProcess = new Process
                {
                    Name = systemProcess.ProcessName,
                    Pid = systemProcess.Id,
                    Path = path
                };

                var settingsProcess = settings.ActiveProcessesList
                                              .FirstOrDefault(x => x.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase));
                if (settingsProcess != null)
                {
                    newProcess.IsEnabled = settingsProcess.IsEnabled;
                    newProcess.ServerInfo = new ServerInfo
                    {
                        ServerIP = settingsProcess.ServerInfo.ServerIP,
                        ServerPort = settingsProcess.ServerInfo.ServerPort,
                        ServerType = settingsProcess.ServerInfo.ServerType
                    };
                }

                guiModelData.ProcessesList.Add(newProcess);
            }

            return guiModelData.ProcessesList;
        }
    }
}