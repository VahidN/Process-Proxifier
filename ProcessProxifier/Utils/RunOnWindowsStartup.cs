using Microsoft.Win32;
using System.Windows.Forms;

namespace ProcessProxifier.Utils
{
    public static class RunOnWindowsStartup
    {
        public static void Do()
        {
            var rkApp = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rkApp != null)
            {
                rkApp.SetValue("ProcessProxifier", Application.ExecutablePath);
            }
        }

        public static void Undo()
        {
            var rkApp = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rkApp != null)
            {
                rkApp.DeleteValue("ProcessProxifier", false);
            }
        }
    }
}