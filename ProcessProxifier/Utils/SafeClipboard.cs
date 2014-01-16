using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace ProcessProxifier.Utils
{
    public static class SafeClipboard
    {
        public static void ClipboardSetText(this string text)
        {
            for (var i = 0; i < 15; i++)
            {
                try
                {
                    Clipboard.SetText(text);
                    break;
                }
                catch (COMException)
                {
                    Thread.Sleep(10);
                }
            }
        }
    }
}