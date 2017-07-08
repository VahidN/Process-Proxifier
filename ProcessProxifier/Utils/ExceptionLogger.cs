using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProcessProxifier.Utils
{
    public static class ExceptionLogger
    {
        // Public Methods (2)

        public static string GetDetailedException(Exception exception)
        {
            var result = new StringBuilder();

            var computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();

            result.AppendLine(string.Format("Application:       {0}", Application.ProductName));
            result.AppendLine(string.Format("Path:              {0}", Application.ExecutablePath));
            result.AppendLine(string.Format("Version:           {0}", Application.ProductVersion));
            result.AppendLine(string.Format("Date:              {0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
            result.AppendLine(string.Format("Computer name:     {0}", SystemInformation.ComputerName));
            result.AppendLine(string.Format("User name:         {0}", SystemInformation.UserName));
            result.AppendLine(string.Format("OSVersion:         {0}", computerInfo.OSVersion));
            result.AppendLine(string.Format("OSPlatform:        {0}", (Environment.Is64BitOperatingSystem ? "X64" : "X86")));
            result.AppendLine(string.Format("OSFullName:        {0}", computerInfo.OSFullName));
            result.AppendLine(string.Format("Culture:           {0}", CultureInfo.CurrentCulture.Name));
            result.AppendLine(string.Format("Resolution:        {0}", SystemInformation.PrimaryMonitorSize));
            result.AppendLine(string.Format("System up time:    {0}", getSystemUpTime()));
            result.AppendLine(string.Format("App up time:       {0}", (DateTime.Now - Process.GetCurrentProcess().StartTime)));
            result.AppendLine(string.Format("Total memory:      {0}Mb", computerInfo.TotalPhysicalMemory / (1024 * 1024)));
            result.AppendLine(string.Format("Available memory:  {0}Mb", computerInfo.AvailablePhysicalMemory / (1024 * 1024)));
            //Getting Available Drive Space
            var currentDrive = DriveInfo.GetDrives().FirstOrDefault(x =>
                String.Equals(x.Name, Application.ExecutablePath.Substring(0, 3), StringComparison.CurrentCultureIgnoreCase));
            if (currentDrive != null)
            {
                result.AppendLine(string.Format("Drive {0}", currentDrive.Name));
                result.AppendLine(string.Format("Volume label: {0}", currentDrive.VolumeLabel));
                result.AppendLine(string.Format("File system: {0}", currentDrive.DriveFormat));
                result.AppendLine(string.Format("Available space to current user: {0} MB", currentDrive.AvailableFreeSpace / (1024 * 1024)));
                result.AppendLine(string.Format("Total available space: {0} MB", currentDrive.TotalFreeSpace / (1024 * 1024)));
                result.AppendLine(string.Format("Total size of drive: {0} MB ", currentDrive.TotalSize / (1024 * 1024)));
            }

            //Get callerInfo
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(2); //caller of LogExceptionToFile
            var methodBase = stackFrame.GetMethod();
            var callingType = methodBase.DeclaringType;
            result.AppendLine(string.Format("Url: {0} -> {1}", callingType.Assembly.Location, callingType.Assembly.FullName));
            result.AppendLine(string.Format("Caller: {0} -> {1}", callingType.FullName, methodBase.Name));


            result.AppendLine("Exception classes: ");
            result.AppendLine(getExceptionTypeStack(exception));
            result.AppendLine("Exception messages: ");
            result.AppendLine(getExceptionMessageStack(exception));
            result.AppendLine("Stack Traces:");
            result.AppendLine(getExceptionCallStack(exception));
            return result.ToString();
        }

        public static void LogExceptionToFile(object exception, string fileName = "ErrosLog.Log")
        {
            try
            {
                string appPath = Path.GetDirectoryName(Application.ExecutablePath);
                var errs = GetDetailedException(exception as Exception);
                File.AppendAllText(
                    appPath + "\\" + fileName,
                    string.Format(@"+-------------------------------------------------------------------+{0}{1}",
                    Environment.NewLine, errs), Encoding.UTF8);
                //todo: send e-mail
            }
            catch
            {
                /*کاری نمی‌شود کرد. بدترین حالت ممکن است*/
            }
        }
        // Private Methods (4)

        private static string getExceptionCallStack(Exception e)
        {
            if (e.InnerException != null)
            {
                var message = new StringBuilder();
                message.AppendLine(getExceptionCallStack(e.InnerException));
                message.AppendLine("--- Next Call Stack:");
                message.AppendLine(e.StackTrace);
                return (message.ToString());
            }
            return e.StackTrace;
        }

        private static string getExceptionMessageStack(Exception e)
        {
            if (e.InnerException != null)
            {
                var message = new StringBuilder();
                message.AppendLine(getExceptionMessageStack(e.InnerException));
                message.AppendLine(string.Format("   {0}", e.Message));
                return (message.ToString());
            }
            return string.Format("   {0}", e.Message);
        }

        private static string getExceptionTypeStack(Exception e)
        {
            if (e.InnerException != null)
            {
                var message = new StringBuilder();
                message.AppendLine(getExceptionTypeStack(e.InnerException));
                message.AppendLine(string.Format("   {0}", e.GetType()));
                return (message.ToString());
            }
            return string.Format("   {0}", e.GetType());
        }

        private static TimeSpan getSystemUpTime()
        {
            using (var upTime = new PerformanceCounter("System", "System Up Time"))
            {
                upTime.NextValue();
                return TimeSpan.FromSeconds(upTime.NextValue());
            }
        }
    }
}