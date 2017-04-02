using System;
using System.Linq;
using ProcessProxifier.Models;
using ProcessProxifier.Utils;

namespace ProcessProxifier.Core
{
    public static class ProcessesListManager
    {
        public static AsyncObservableCollection<Process> UpdateProcesses(ProxifierSettings guiModelData, ProxifierSettings settings)
        {
            var processesList = System.Diagnostics.Process.GetProcesses().OrderBy(x => x.ProcessName).ToList();

            var finishedProcesses = guiModelData.ProcessesList
                                                .Where(x => !processesList.Select(p => p.Id).Contains(x.Pid)
                                                            && !x.IsEnabled)
                                                .ToList();
            foreach (var process in finishedProcesses)
            {
                guiModelData.ProcessesList.Remove(process);
            }

            var newProcesses = processesList.Where(x => !guiModelData.ProcessesList.Select(p => p.Pid).Contains(x.Id)).ToList();

            foreach (var process in newProcesses)
            {
                var path = getPath(process);
                var newProcess = new Process
                {
                    Name = process.ProcessName,
                    Pid = process.Id,
                    Path = path
                };

                var settingsProcess = settings.ProcessesList
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

        private static string getPath(System.Diagnostics.Process process)
        {
            try
            {
                return process.MainModule.FileName;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}