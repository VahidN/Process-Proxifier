using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace ProcessProxifier.Models
{
    public class Process : INotifyPropertyChanged
    {
        bool _isEnabled;
        string _name;
        int _pid;
        ServerInfo _serverInfo = new ServerInfo();

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

        [JsonIgnore]
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void notifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{Name}, {Path}";
        }

        public override bool Equals(object obj)
        {
            var process = obj as Process;
            if (process == null)
                return false;

            return this.Path.Equals(process.Path, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + Path.GetHashCode();
                return hash;
            }
        }
    }
}