using System.Windows;
using ProcessProxifier.Utils;

namespace ProcessProxifier
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // Enable "minimize to tray" behavior for this Window
            MinimizeToTray.Enable(this);
            this.Loaded += mainWindowLoaded;
        }

        static void mainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;            
        }
    }
}