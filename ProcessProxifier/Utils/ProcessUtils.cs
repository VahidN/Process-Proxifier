namespace ProcessProxifier.Utils
{
    public static class ProcessUtils
    {
        public static string GetPath(this System.Diagnostics.Process process)
        {
            try
            {
                return process.MainModule.FileName;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}