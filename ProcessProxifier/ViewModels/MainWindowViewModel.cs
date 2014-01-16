﻿using System;
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
        #region Fields (3)

        readonly ProxyRouter _poxyRouter = new ProxyRouter();
        readonly ProxifierSettings _settings;
        readonly SimpleTaskScheduler _taskScheduler = new SimpleTaskScheduler();

        #endregion Fields

        #region Constructors (1)

        public MainWindowViewModel()
        {
            setupData();
            setupCommands();
            _settings = SettingsManager.LoadSettings(GuiModelData);
            manageAppExit();
            Task.Factory.StartNew(() => doStart(string.Empty));
            initTaskScheduler();
        }

        #endregion Constructors

        #region Properties (10)

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

        #endregion Properties

        #region Methods (14)

        // Private Methods (14) 

        bool canDoStart(string data)
        {
            return GuiModelData.IsEnabled;
        }

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
            (string.Format("{0}\t{1}", GuiModelData.SelectedRoutedConnection.ProcessName, GuiModelData.SelectedRoutedConnection.Url)).ClipboardSetText();
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
            SettingsManager.SaveSettings(GuiModelData);
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
            SettingsManager.SaveSettings(GuiModelData);
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
                if (GuiModelData.RoutedConnectionsList.Count > 1000)
                {
                    GuiModelData.RoutedConnectionsList.Clear();
                }
            };
        }

        private void manageAppExit()
        {
            Application.Current.Exit += currentExit;
            Application.Current.SessionEnding += currentSessionEnding;
        }

        private void setupCommands()
        {
            DoStart = new DelegateCommand<string>(doStart, canDoStart);
            DoStop = new DelegateCommand<string>(doStop, data => true);
            DoSave = new DelegateCommand<string>(data => SettingsManager.SaveSettings(GuiModelData), data => true);
            DoClearLogs = new DelegateCommand<string>(data => GuiModelData.RoutedConnectionsList.Clear(), data => true);
            DoClearLogsList = new DelegateCommand<string>(data => GuiModelData.RoutedConnectionsList.Clear(), data => true);
            DoUseDefaultSettings = new DelegateCommand<string>(doUseDefaultSettings, data => true);
            DoRefresh = new DelegateCommand<string>(data => ProcessesListManager.UpdateProcesses(GuiModelData, _settings), data => true);
            DoCopySelectedLine = new DelegateCommand<string>(doCopySelectedLine, data => true);
            DoCopyAllLines = new DelegateCommand<string>(doCopyAllLines, data => true);
        }

        private void setupData()
        {
            GuiModelData = new ProxifierSettings();
            GuiModelData.PropertyChanged += guiModelDataPropertyChanged;
            GuiModelData.ProcessesListDataView = CollectionViewSource.GetDefaultView(GuiModelData.ProcessesList);
        }

        #endregion Methods
    }
}