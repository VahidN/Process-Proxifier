using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

namespace ProcessProxifier.Utils
{
    //from http://blogs.msdn.com/delay/archive/2009/08/31/get-out-of-the-way-with-the-tray-minimize-to-tray-sample-implementation-for-wpf.aspx
    /// <summary>
    /// Class implementing support for "minimize to tray" functionality.
    /// </summary>
    public static class MinimizeToTray
    {
        private static MinimizeToTrayInstance _minimizeToTrayInstance;
        /// <summary>
        /// Enables "minimize to tray" behavior for the specified Window.
        /// </summary>
        /// <param name="window">Window to enable the behavior for.</param>
        public static void Enable(Window window)
        {
            // No need to track this instance; its event handlers will keep it alive
            _minimizeToTrayInstance = new MinimizeToTrayInstance(window);
        }

        public static void ShowBalloonTip(string txt, ToolTipIcon icon)
        {
            DispatcherHelper.DispatchAction(() => _minimizeToTrayInstance.NotifyIcon.ShowBalloonTip(500, null, txt, icon));
        }

        /// <summary>
        /// Class implementing "minimize to tray" functionality for a Window instance.
        /// </summary>
        private class MinimizeToTrayInstance
        {
            private readonly Window _window;
            private NotifyIcon _notifyIcon;
            private bool _balloonShown;

            public NotifyIcon NotifyIcon
            {
                get { return _notifyIcon; }
            }

            /// <summary>
            /// Initializes a new instance of the MinimizeToTrayInstance class.
            /// </summary>
            /// <param name="window">Window instance to attach to.</param>
            public MinimizeToTrayInstance(Window window)
            {
                Debug.Assert(window != null, "window parameter is null.");
                _window = window;
                _window.StateChanged += handleStateChanged;
            }

            /// <summary>
            /// Handles the Window's StateChanged event.
            /// </summary>
            /// <param name="sender">Event source.</param>
            /// <param name="e">Event arguments.</param>
            private void handleStateChanged(object sender, EventArgs e)
            {
                if (_notifyIcon == null)
                {
                    // Initialize NotifyIcon instance "on demand"
                    _notifyIcon = new NotifyIcon
                    {
                        Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location)
                    };
                    _notifyIcon.MouseClick += handleNotifyIconOrBalloonClicked;
                    _notifyIcon.BalloonTipClicked += handleNotifyIconOrBalloonClicked;
                }
                // Update copy of Window Title in case it has changed
                _notifyIcon.Text = _window.Title;

                // Show/hide Window and NotifyIcon
                var minimized = (_window.WindowState == WindowState.Minimized);
                _window.ShowInTaskbar = !minimized;
                _notifyIcon.Visible = minimized;
                if (minimized && !_balloonShown)
                {
                    // If this is the first time minimizing to the tray, show the user what happened
                    //_notifyIcon.ShowBalloonTip(1000, null, _window.Title, ToolTipIcon.None);
                    _balloonShown = true;
                }
            }

            /// <summary>
            /// Handles a click on the notify icon or its balloon.
            /// </summary>
            /// <param name="sender">Event source.</param>
            /// <param name="e">Event arguments.</param>
            private void handleNotifyIconOrBalloonClicked(object sender, EventArgs e)
            {
                // Restore the Window
                _window.WindowState = WindowState.Normal;
            }
        }
    }
}