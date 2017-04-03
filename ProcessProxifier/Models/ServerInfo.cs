using System.ComponentModel;

namespace ProcessProxifier.Models
{
    public class ServerInfo : INotifyPropertyChanged
    {
        ServerType _serverType;
        string _serverIP = string.Empty;
        int _serverPort;

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