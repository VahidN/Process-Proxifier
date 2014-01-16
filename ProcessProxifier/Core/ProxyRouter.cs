using System.Linq;
using System.Net.Security;
using System.Threading;
using Fiddler;
using ProcessProxifier.Models;
using ProcessProxifier.Utils;

namespace ProcessProxifier.Core
{
    using System.Globalization;

    public class ProxyRouter
    {
        #region Properties (4)

        public ServerInfo DefaultServerInfo { set; get; }

        public int FiddlerPort { set; get; }

        public AsyncObservableCollection<Process> ProcessesList { set; get; }

        public AsyncObservableCollection<RoutedConnection> RoutedConnectionsList { set; get; }

        #endregion Properties

        #region Methods (5)

        // Public Methods (2) 

        public void Shutdown()
        {
            FiddlerApplication.BeforeRequest -= beforeRequest;
            FiddlerApplication.OnValidateServerCertificate -= onValidateServerCertificate;

            if (FiddlerApplication.oProxy != null)
                FiddlerApplication.oProxy.Detach();

            FiddlerApplication.Shutdown();

            Thread.Sleep(1000);
        }

        public void Start()
        {
            FiddlerApplication.BeforeRequest += beforeRequest;
            FiddlerApplication.OnValidateServerCertificate += onValidateServerCertificate;

            FiddlerApplication.Startup(FiddlerPort,
                FiddlerCoreStartupFlags.RegisterAsSystemProxy |
                FiddlerCoreStartupFlags.MonitorAllConnections |
                FiddlerCoreStartupFlags.CaptureFTP);
        }
        // Private Methods (3) 

        void beforeRequest(Session oSession)
        {
            var process = ProcessesList.FirstOrDefault(p => p.Pid == oSession.LocalProcessID && p.IsEnabled);
            if (process == null)
                return;

            var useDefaultServerInfo = string.IsNullOrWhiteSpace(process.ServerInfo.ServerIP);
            if (useDefaultServerInfo)
            {
                oSession["X-OverrideGateway"] = (DefaultServerInfo.ServerType == ServerType.Socks ? "socks=" : "") + DefaultServerInfo.ServerIP + ":" + DefaultServerInfo.ServerPort;
            }
            else
            {
                oSession["X-OverrideGateway"] = (process.ServerInfo.ServerType == ServerType.Socks ? "socks=" : "") + process.ServerInfo.ServerIP + ":" + process.ServerInfo.ServerPort;
            }

            RoutedConnectionsList.Add(new RoutedConnection
            {
                ProcessPid = oSession.LocalProcessID,
                Url = oSession.fullUrl,
                ProcessName = getProcessName(oSession),
                ProcessPath = process.Path
            });
        }

        private string getProcessName(Session oSession)
        {
            var process = ProcessesList.FirstOrDefault(x => x.Pid == oSession.LocalProcessID);
            return process == null ? oSession.LocalProcessID.ToString(CultureInfo.InvariantCulture) : process.Name;
        }

        static void onValidateServerCertificate(object sender, ValidateServerCertificateEventArgs e)
        {
            if (SslPolicyErrors.None == e.CertificatePolicyErrors)
                return;

            e.ValidityState = CertificateValidity.ForceValid;
        }

        #endregion Methods
    }
}