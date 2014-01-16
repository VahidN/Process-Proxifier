using System.ComponentModel;

namespace ProcessProxifier.Models
{
    public class ServerInfo : INotifyPropertyChanged
    {
        #region Fields

        ServerType _serverType;
        string _serverIP = string.Empty;
        int _serverPort;

        #endregion Fields

        #region Properties (4)

        public ServerType ServerType
        {
            get { return _serverType; }
            set
            {
                _serverType = value;
                notifyPropertyChanged("ServerType");
            }
        }

        public string ServerIP
        {
            get { return _serverIP; }
            set
            {
                _serverIP = value;
                notifyPropertyChanged("ServerIP");
            }
        }

        public int ServerPort
        {
            get { return _serverPort; }
            set
            {
                _serverPort = value;
                notifyPropertyChanged("ServerPort");
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