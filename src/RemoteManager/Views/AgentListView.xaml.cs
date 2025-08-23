using System.Windows.Controls;
using RemoteManager.ViewModels;

namespace RemoteManager.Views;

public partial class AgentListView : UserControl
{
    public AgentListView(AgentListViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
