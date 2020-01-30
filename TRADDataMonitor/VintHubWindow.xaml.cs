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
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
