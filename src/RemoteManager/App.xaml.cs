using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RemoteManager.Services;
using RemoteManager.ViewModels;
using RemoteManager.Views;
using Serilog;

namespace RemoteManager
{
    public partial class App : Application
    {
        public IHost Host { get; }

        public App()
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((ctx, cfg) =>
                {
                    IHostEnvironment env = ctx.HostingEnvironment;
                    cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                       .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                       .AddEnvironmentVariables();
                })
                .UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration))
                .ConfigureServices((ctx, services) =>
                {
                    string endpoint = ctx.Configuration.GetValue<string>("Client:DefaultAgentEndpoint")!;
                    services.AddSingleton<IDiscoveryService, DiscoveryService>();
                    services.AddSingleton(sp =>
                    {
                        var cfg = sp.GetRequiredService<IConfiguration>();
                        string? path = cfg.GetValue<string>("Paths:TrustedAgents");
                        return new RemoteManager.Security.TrustedAgentsStore(path);
                    });
                    services.AddSingleton<IAgentRegistry, AgentRegistry>();
                    services.AddSingleton<IGrpcConnectionFactory, GrpcConnectionFactory>();
                    services.AddSingleton<ISystemClient>(sp => new SystemClient(sp.GetRequiredService<IGrpcConnectionFactory>().Create(endpoint)));
                    services.AddSingleton<IFileClient>(sp => new FileClient(sp.GetRequiredService<IGrpcConnectionFactory>().Create(endpoint)));
                    services.AddSingleton<IProcessClient>(sp => new ProcessClient(sp.GetRequiredService<IGrpcConnectionFactory>().Create(endpoint)));
                    services.AddSingleton<ShellViewModel>();
                    services.AddSingleton<AgentListViewModel>();
                    services.AddSingleton<CommanderViewModel>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();
            Host.Start();

            IConfiguration cfg = Host.Services.GetRequiredService<IConfiguration>();
            ILogger<App> logger = Host.Services.GetRequiredService<ILogger<App>>();
            if (!cfg.GetValue<bool>("Server:Enabled"))
            {
                logger.LogInformation("Server startup skipped (dev)");
            }
            if (!cfg.GetValue<bool>("Discovery:Enabled") || !cfg.GetValue<bool>("Discovery:ReceiveEnabled"))
            {
                logger.LogInformation("Discovery receive disabled (dev)");
            }
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
}

