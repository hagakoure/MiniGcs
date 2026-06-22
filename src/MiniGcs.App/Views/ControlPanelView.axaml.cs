using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MiniGcs.App.Views;

public partial class ControlPanelView : UserControl
{
    public ControlPanelView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}