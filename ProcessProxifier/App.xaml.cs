using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ProcessProxifier.Utils;

namespace ProcessProxifier
{
    public partial class App
    {
        void checkSingleInst()
        {
            //WPF Single Instance Application
            var process = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (process.Length <= 1) return;
            MessageBox.Show("ProcessProxifier is already running ...", "ProcessProxifier", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Shutdown();
        }

        public App()
        {
            this.DispatcherUnhandledException += appDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += currentDomainUnhandledException;
            this.Startup += appStartup;
            this.Deactivated += appDeactivated;
            checkSingleInst();
        }

        static void appDeactivated(object sender, EventArgs e)
        {
            Memory.ReEvaluatedWorkingSet();
        }

        void appStartup(object sender, StartupEventArgs e)
        {
            reducingCpuConsumptionForAnimations();
        }

        static void reducingCpuConsumptionForAnimations()
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                 typeof(Timeline),
                 new FrameworkPropertyMetadata { DefaultValue = 20 }
                 );
        }

        static void currentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            ExceptionLogger.LogExceptionToFile(ex);
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void appDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ExceptionLogger.LogExceptionToFile(e.Exception);
            MessageBox.Show(e.Exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}