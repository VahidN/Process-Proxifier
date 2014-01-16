using System;
using System.Diagnostics;

namespace ProcessProxifier.Utils
{
    public static class Memory
    {
        public static void ReEvaluatedWorkingSet()
        {
            try
            {
                var loProcess = Process.GetCurrentProcess();
                loProcess.MaxWorkingSet = (IntPtr)((int)loProcess.MaxWorkingSet + 1);
            }
            catch
            { /* مهم نیست. شخص دسترسی لازم را ندارد */ }
        }
    }
}