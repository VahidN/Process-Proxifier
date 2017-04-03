using System.ComponentModel;
using Newtonsoft.Json;
using ProcessProxifier.Utils;

namespace ProcessProxifier.Models
{
    public class ProxifierSettings : INotifyPropertyChanged
    {
        bool _areAllChecked;
        bool _isEnabled = true;
        AsyncObservableCollection<Process> _processesList = new AsyncObservableCollection<Process>();
        ICollectionView _processesListDataView;
        int _proxifierPort = 5656;
        AsyncObservableCollection<RoutedConnection> _routedConnectionsList = new AsyncObservableCollection<RoutedConnection>();
        bool _runOnStartup = true;
        string _searchText;
        Process _selectedProcess;
        RoutedConnection _selectedRoutedConnection;
        ServerInfo _serverInfo = new ServerInfo();

        public ProxifierSettings()
        {
            ActiveProcessesList = new AsyncObservableCollection<Process>();
        }

        public bool AreAllChecked
        {
            get { return _areAllChecked; }
            set
            {
                _areAllChecked = value;
                notifyPropertyChanged("AreAllChecked");

                foreach (var item in ProcessesList)
                    item.IsEnabled = value;
            }
        }

        public ServerInfo DefaultServerInfo
        {
            get { return _serverInfo; }
            set
            {
                _serverInfo = value;
                notifyPropertyChanged("DefaultServerInfo");
            }
        }

        [JsonIgnore]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                notifyPropertyChanged("IsEnabled");
            }
        }

        [JsonIgnore]
        public AsyncObservableCollection<Process> ProcessesList
        {
            get { return _processesList; }
            set
            {
                _processesList = value;
                notifyPropertyChanged("ProcessesList");
            }
        }

        public AsyncObservableCollection<Process> ActiveProcessesList
        {
            get; set;
        }

        [JsonIgnore]
        public ICollectionView ProcessesListDataView
        {
            get { return _processesListDataView; }
            set
            {
                _processesListDataView = value;
                notifyPropertyChanged("ProcessesListDataView");
            }
        }

        public int ProxifierPort
        {
            get { return _proxifierPort; }
            set
            {
                _proxifierPort = value;
                notifyPropertyChanged("ProxifierPort");
            }
        }

        [JsonIgnore]
        public AsyncObservableCollection<RoutedConnection> RoutedConnectionsList
        {
            get { return _routedConnectionsList; }
            set
            {
                _routedConnectionsList = value;
                notifyPropertyChanged("RoutedConnectionsList");
            }
        }

        public bool RunOnStartup
        {
            get { return _runOnStartup; }
            set
            {
                _runOnStartup = value;
                notifyPropertyChanged("RunOnStartup");
            }
        }

        [JsonIgnore]
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                notifyPropertyChanged("SearchText");
            }
        }

        [JsonIgnore]
        public Process SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                _selectedProcess = value;
                notifyPropertyChanged("SelectedProcess");
            }
        }

        [JsonIgnore]
        public RoutedConnection SelectedRoutedConnection
        {
            get { return _selectedRoutedConnection; }
            set
            {
                _selectedRoutedConnection = value;
                notifyPropertyChanged("SelectedRoutedConnection");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void notifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}