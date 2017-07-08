using System;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading;
using Fiddler;
using ProcessProxifier.Models;
using ProcessProxifier.Utils;

namespace ProcessProxifier.Core
{
    using System.Globalization;

    public class ProxyRouter
    {
        public ServerInfo DefaultServerInfo { set; get; }

        public int FiddlerPort { set; get; }

        public AsyncObservableCollection<Process> ProcessesList { set; get; }

        public AsyncObservableCollection<RoutedConnection> RoutedConnectionsList { set; get; }


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


        private static void onValidateServerCertificate(object sender, ValidateServerCertificateEventArgs e)
        {
            if (SslPolicyErrors.None == e.CertificatePolicyErrors)
                return;

            e.ValidityState = CertificateValidity.ForceValid;
        }

        private static void setBasicAuthenticationHeaders(Session oSession, string userCredentials)
        {
            var base64UserCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(userCredentials));
            oSession.RequestHeaders["Proxy-Authorization"] = $"Basic {base64UserCredentials}";
        }

        private void beforeRequest(Session oSession)
        {
            var process = ProcessesList.FirstOrDefault(p => p.Pid == oSession.LocalProcessID && p.IsEnabled);
            if (process == null)
            {
                return;
            }

            var processServerInfo = process.ServerInfo;
            var useDefaultServerInfo = string.IsNullOrWhiteSpace(processServerInfo.ServerIP);
            if (useDefaultServerInfo)
            {
                var serverType = DefaultServerInfo.ServerType == ServerType.Socks ? "socks=" : "";
                oSession["X-OverrideGateway"] = $"{serverType}{DefaultServerInfo.ServerIP}:{DefaultServerInfo.ServerPort}";

                if (!string.IsNullOrWhiteSpace(DefaultServerInfo.Username) &&
                    !string.IsNullOrWhiteSpace(DefaultServerInfo.Password))
                {
                    var userCredentials = $"{DefaultServerInfo.Username}:{DefaultServerInfo.Password}";
                    setBasicAuthenticationHeaders(oSession, userCredentials);
                }
            }
            else
            {
                var serverType = processServerInfo.ServerType == ServerType.Socks ? "socks=" : "";
                oSession["X-OverrideGateway"] = $"{serverType}{processServerInfo.ServerIP}:{processServerInfo.ServerPort}";

                if (!string.IsNullOrWhiteSpace(processServerInfo.Username) &&
                    !string.IsNullOrWhiteSpace(processServerInfo.Password))
                {
                    var userCredentials = $"{processServerInfo.Username}:{processServerInfo.Password}";
                    setBasicAuthenticationHeaders(oSession, userCredentials);
                }
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
    }
}