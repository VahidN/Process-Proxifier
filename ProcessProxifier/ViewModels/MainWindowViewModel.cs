using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ProcessProxifier.Core;
using ProcessProxifier.Models;
using ProcessProxifier.Utils;

namespace ProcessProxifier.ViewModels
{
    public class MainWindowViewModel
    {
        readonly ProxyRouter _poxyRouter = new ProxyRouter();
        readonly SimpleTaskScheduler _taskScheduler = new SimpleTaskScheduler();
        ProxifierSettings _settings;

        public MainWindowViewModel()
        {
            setupData();
            setupCommands();
            loadSettings();
            manageAppExit();
            Task.Factory.StartNew(() => doStart(string.Empty));
            initTaskScheduler();
        }

        public DelegateCommand<string> DoClearLogs { set; get; }

        public DelegateCommand<string> DoClearLogsList { set; get; }

        public DelegateCommand<string> DoCopyAllLines { set; get; }

        public DelegateCommand<string> DoCopySelectedLine { set; get; }

        public DelegateCommand<string> DoRefresh { set; get; }

        public DelegateCommand<string> DoSave { set; get; }

        public DelegateCommand<string> DoStart { set; get; }

        public DelegateCommand<string> DoStop { set; get; }

        public DelegateCommand<string> DoUseDefaultSettings { set; get; }

        public ProxifierSettings GuiModelData { set; get; }

        bool canDoStart(string data)
        {
            return GuiModelData.IsEnabled;
        }

        // Private Methods (14)
        void currentExit(object sender, ExitEventArgs e)
        {
            exit();
        }

        void currentSessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            exit();
        }

        void doCopyAllLines(string data)
        {
            var lines = new StringBuilder();
            foreach (var item in GuiModelData.RoutedConnectionsList)
            {
                lines.AppendLine(string.Format("{0}\t{1}", item.ProcessName, item.Url));
            }

            lines.ToString().ClipboardSetText();
        }

        void doCopySelectedLine(string data)
        {
            if (GuiModelData.SelectedRoutedConnection == null) return;
            string.Format("{0}\t{1}", GuiModelData.SelectedRoutedConnection.ProcessName, GuiModelData.SelectedRoutedConnection.Url).ClipboardSetText();
        }

        void doStart(string data)
        {
            ProcessesListManager.UpdateProcesses(GuiModelData, _settings);

            if (Designer.IsInDesignModeStatic)
                return;

            if (string.IsNullOrWhiteSpace(GuiModelData.DefaultServerInfo.ServerIP))
                return;

            _poxyRouter.FiddlerPort = GuiModelData.ProxifierPort;
            _poxyRouter.DefaultServerInfo = new ServerInfo
            {
                ServerIP = GuiModelData.DefaultServerInfo.ServerIP,
                ServerPort = GuiModelData.DefaultServerInfo.ServerPort,
                ServerType = GuiModelData.DefaultServerInfo.ServerType
            };
            _poxyRouter.ProcessesList = GuiModelData.ProcessesList;
            _poxyRouter.RoutedConnectionsList = GuiModelData.RoutedConnectionsList;
            _poxyRouter.Start();
            GuiModelData.IsEnabled = false;
        }

        void doStop(string data)
        {
            _poxyRouter.Shutdown();
            GuiModelData.RoutedConnectionsList.Clear();
            saveSettings();
            GuiModelData.IsEnabled = true;
        }

        void doUseDefaultSettings(string data)
        {
            if (GuiModelData.SelectedProcess == null)
                return;

            GuiModelData.SelectedProcess.ServerInfo.ServerType = GuiModelData.DefaultServerInfo.ServerType;
            GuiModelData.SelectedProcess.ServerInfo.ServerIP = GuiModelData.DefaultServerInfo.ServerIP;
            GuiModelData.SelectedProcess.ServerInfo.ServerPort = GuiModelData.DefaultServerInfo.ServerPort;
        }

        private void exit()
        {
            saveSettings();
            _poxyRouter.Shutdown();
            _taskScheduler.Stop();
        }

        void guiModelDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SearchText":
                    GuiModelData.ProcessesListDataView.Filter = row =>
                    {
                        if (row == null) return false;
                        var process = row as Process;
                        return process != null &&
                               process.Name.StartsWith(GuiModelData.SearchText, StringComparison.InvariantCultureIgnoreCase);
                    };
                    break;
            }
        }

        private void initTaskScheduler()
        {
            _taskScheduler.Start();
            _taskScheduler.DoWork = () =>
            {
                try
                {
                    ProcessesListManager.UpdateProcesses(GuiModelData, _settings);

                    if (GuiModelData.RoutedConnectionsList.Count > 500)
                    {
                        GuiModelData.RoutedConnectionsList.Clear();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogExceptionToFile(ex);
                }
            };
        }

        private void loadSettings()
        {
            _settings = SettingsManager.LoadSettings(GuiModelData);
        }

        private void manageAppExit()
        {
            Application.Current.Exit += currentExit;
            Application.Current.SessionEnding += currentSessionEnding;
        }

        private void saveSettings()
        {
            SettingsManager.SaveSettings(GuiModelData, _settings);
        }

        private void setupCommands()
        {
            DoStart = new DelegateCommand<string>(doStart, canDoStart);
            DoStop = new DelegateCommand<string>(doStop, data => true);
            DoSave = new DelegateCommand<string>(data => saveSettings(), data => true);
            DoClearLogs = new DelegateCommand<string>(data => GuiModelData.RoutedConnectionsList.Clear(), data => true);
            DoClearLogsList = new DelegateCommand<string>(data => GuiModelData.RoutedConnectionsList.Clear(), data => true);
            DoUseDefaultSettings = new DelegateCommand<string>(doUseDefaultSettings, data => true);
            DoRefresh = new DelegateCommand<string>(data => ProcessesListManager.UpdateProcesses(GuiModelData, _settings), data => true);
            DoCopySelectedLine = new DelegateCommand<string>(doCopySelectedLine, data => true);
            DoCopyAllLines = new DelegateCommand<string>(doCopyAllLines, data => true);
        }

        private void setupData()
        {
            GuiModelData = new ProxifierSettings
            {
                RoutedConnectionsList = new AsyncObservableCollection<RoutedConnection>(),
                ProcessesList = new AsyncObservableCollection<Process>()
            };
            GuiModelData.PropertyChanged += guiModelDataPropertyChanged;
            GuiModelData.ProcessesListDataView = CollectionViewSource.GetDefaultView(GuiModelData.ProcessesList);
        }
    }
}