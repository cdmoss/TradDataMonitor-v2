using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TRADDataMonitor
{
    public class VintHubWindow : Window
    {
        public VintHubWindow()
        {
            this.InitializeComponent();
            VintHubViewModel vhvm = new VintHubViewModel();

#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
    // mw._mainWindow, mw, this
}
