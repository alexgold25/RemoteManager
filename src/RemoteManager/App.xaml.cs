using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RemoteManager.Services;
using RemoteManager.ViewModels;
using RemoteManager.Views;

namespace RemoteManager;

public partial class App : Application
{
    public IHost Host { get; }

    public App()
    {
        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IDiscoveryService, DiscoveryService>();
                services.AddSingleton<IAgentRegistry, AgentRegistry>();
                services.AddSingleton<IGrpcConnectionFactory, GrpcConnectionFactory>();
                services.AddSingleton<ISystemClient>(sp => new SystemClient(sp.GetRequiredService<IGrpcConnectionFactory>().Create("https://localhost:8443")));
                services.AddSingleton<IFileClient>(sp => new FileClient(sp.GetRequiredService<IGrpcConnectionFactory>().Create("https://localhost:8443")));
                services.AddSingleton<IProcessClient>(sp => new ProcessClient(sp.GetRequiredService<IGrpcConnectionFactory>().Create("https://localhost:8443")));
                services.AddSingleton<ShellViewModel>();
                services.AddSingleton<AgentListViewModel>();
                services.AddSingleton<CommanderViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();
        Host.Start();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Host.Services.GetRequiredService<MainWindow>().Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Host.Dispose();
        base.OnExit(e);
    }
}
