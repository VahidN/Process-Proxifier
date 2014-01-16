using System.ComponentModel;
using System.Xml.Serialization;

namespace ProcessProxifier.Models
{
    public class Process : INotifyPropertyChanged
    {
        #region Fields (4)

        bool _isEnabled;
        string _name;
        int _pid;
        ServerInfo _serverInfo = new ServerInfo();

        #endregion Fields

        #region Properties (5)

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                notifyPropertyChanged("IsEnabled");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                notifyPropertyChanged("Name");
            }
        }

        public string Path { set; get; }

        [XmlIgnore]
        public int Pid
        {
            get { return _pid; }
            set
            {
                _pid = value;
                notifyPropertyChanged("Pid");
            }
        }

        public ServerInfo ServerInfo
        {
            get { return _serverInfo; }
            set
            {
                _serverInfo = value;
                notifyPropertyChanged("ServerInfo");
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