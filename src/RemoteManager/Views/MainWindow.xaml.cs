using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RemoteManager.ViewModels;

namespace RemoteManager.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = ((App)Application.Current).Host.Services.GetRequiredService<MainWindowViewModel>();
    }
}
