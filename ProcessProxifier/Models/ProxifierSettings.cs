using System.ComponentModel;
using System.Xml.Serialization;
using ProcessProxifier.Utils;

namespace ProcessProxifier.Models
{
    public class ProxifierSettings : INotifyPropertyChanged
    {
        #region Fields (11)

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

        #endregion Fields

        #region Properties (11)

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

        [XmlIgnore]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                notifyPropertyChanged("IsEnabled");
            }
        }

        public AsyncObservableCollection<Process> ProcessesList
        {
            get { return _processesList; }
            set
            {
                _processesList = value;
                notifyPropertyChanged("ProcessesList");
            }
        }

        [XmlIgnore]
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

        [XmlIgnore]
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

        [XmlIgnore]
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                notifyPropertyChanged("SearchText");
            }
        }

        [XmlIgnore]
        public Process SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                _selectedProcess = value;
                notifyPropertyChanged("SelectedProcess");
            }
        }

        [XmlIgnore]
        public RoutedConnection SelectedRoutedConnection
        {
            get { return _selectedRoutedConnection; }
            set
            {
                _selectedRoutedConnection = value;
                notifyPropertyChanged("SelectedRoutedConnection");
            }
        }

        #endregion Properties



        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void notifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}