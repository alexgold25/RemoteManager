using System.Windows.Controls;
using RemoteManager.ViewModels;

namespace RemoteManager.Views;

public partial class CommanderView : UserControl
{
    public CommanderView(CommanderViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
