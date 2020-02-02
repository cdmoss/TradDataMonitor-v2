using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TRADDataMonitor
{
    public class GraphWindow : Window
    {
        public GraphWindow()
        {
            this.InitializeComponent();
            this.DataContext = new GraphWindowViewModel(this);
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