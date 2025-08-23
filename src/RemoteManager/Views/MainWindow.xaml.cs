using System.Windows;
using RemoteManager.ViewModels;

namespace RemoteManager.Views;

public partial class MainWindow : Window
{
    public MainWindow(ShellViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
