using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RM.Shared;
using RMAgent.Services;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((ctx, cfg) => cfg.WriteTo.File("logs/agent.log", rollingInterval: RollingInterval.Day))
    .ConfigureServices((ctx, services) =>
    {
        services.AddGrpc();
        services.AddSingleton(new DiscoverySocket(8443));
        services.AddHostedService<DiscoveryWorker>();
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureKestrel(options =>
        {
            var cert = DevCertLoader.Load();
            options.ListenAnyIP(8443, o =>
            {
                o.Protocols = HttpProtocols.Http2;
                o.UseHttps(TlsOptionsFactory.CreateHttp2(cert));
            });
        });
        webBuilder.Configure(app =>
        {
            app.MapGrpcService<SystemServiceImpl>();
            app.MapGrpcService<FilesServiceImpl>();
            app.MapGrpcService<ProcessesServiceImpl>();
        });
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Listening on 8443 TCP");

await host.RunAsync();
