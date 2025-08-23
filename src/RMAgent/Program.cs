using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using RM.Shared;
using RMAgent; // DiscoveryWorker
using RMAgent.Services;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
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
        services.AddGrpc();
        if (ctx.Configuration.GetValue<bool>("Network:EnableDiscovery"))
        {
            int port = ctx.Configuration.GetValue<int>("Network:Port");
            services.AddSingleton(new DiscoverySocket(port));
            services.AddHostedService<DiscoveryWorker>();
        }
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureKestrel((ctx, options) =>
        {
            IConfiguration cfg = ctx.Configuration;
            X509Certificate2 cert = DevCertLoader.Load(cfg);
            IPAddress ip = IPAddress.Parse(cfg["Network:BindIp"]!);
            int port = cfg.GetValue<int>("Network:Port");
            options.Listen(ip, port, o =>
            {
                o.Protocols = HttpProtocols.Http2;
                o.UseHttps(TlsOptionsFactory.CreateHttp2(cert));
            });
            if (cfg.GetValue<bool>("Network:EnableUdpQuic"))
            {
                options.Listen(ip, port, o =>
                {
                    o.Protocols = HttpProtocols.Http3;
                    o.UseHttps(TlsOptionsFactory.CreateHttp3(cert));
                });
            }
        });
        webBuilder.Configure(app =>
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<SystemServiceImpl>();
                endpoints.MapGrpcService<FilesServiceCore>();
                endpoints.MapGrpcService<ProcessesServiceImpl>();
            });
        });
    })
    .Build();

IConfiguration cfg = host.Services.GetRequiredService<IConfiguration>();
ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
string? ip = cfg["Network:BindIp"];
int port = cfg.GetValue<int>("Network:Port");
logger.LogInformation("Listening on {ip}:{port} (h2)", ip, port);
if (cfg.GetValue<bool>("Network:EnableUdpQuic"))
{
    logger.LogInformation("Listening on {ip}:{port} (h3)", ip, port);
}

await host.RunAsync();
