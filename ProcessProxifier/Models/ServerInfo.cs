using System.ComponentModel;

namespace ProcessProxifier.Models
{
    public class ServerInfo : INotifyPropertyChanged
    {
        string _password = string.Empty;
        string _serverIP = string.Empty;
        int _serverPort;
        ServerType _serverType;
        string _username = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                notifyPropertyChanged("Password");
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

        public ServerType ServerType
        {
            get { return _serverType; }
            set
            {
                _serverType = value;
                notifyPropertyChanged("ServerType");
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                notifyPropertyChanged("Username");
            }
        }

        private void notifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}