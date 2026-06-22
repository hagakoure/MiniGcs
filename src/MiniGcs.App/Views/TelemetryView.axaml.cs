using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MiniGcs.App.Views;

public partial class TelemetryView : UserControl
{
    public TelemetryView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}