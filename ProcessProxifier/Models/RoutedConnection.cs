
namespace ProcessProxifier.Models
{
    public class RoutedConnection
    {
        public string ProcessName { set; get; }
        public int ProcessPid { set; get; }
        public string Url { set; get; }
        public string ProcessPath { set; get; }
    }
}