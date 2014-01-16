using System;
using System.Windows;
using System.Windows.Threading;

namespace ProcessProxifier.Utils
{
    public static class DispatcherHelper
    {
        public static void DispatchAction(Action func)
        {
            var dispatcher = Application.Current != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;

            if (func == null || dispatcher == null)
                return;

            dispatcher.Invoke(DispatcherPriority.ApplicationIdle, func);
        }
    }
}