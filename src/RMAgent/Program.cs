using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RM.Shared;
using RMAgent.Services;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((ctx, cfg) =>
    {
        var env = ctx.HostingEnvironment;
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
            var port = ctx.Configuration.GetValue<int>("Network:Port");
            services.AddSingleton(new DiscoverySocket(port));
            services.AddHostedService<DiscoveryWorker>();
        }
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureKestrel((ctx, options) =>
        {
            var cfg = ctx.Configuration;
            var cert = DevCertLoader.Load(cfg);
            var ip = IPAddress.Parse(cfg["Network:BindIp"]!);
            var port = cfg.GetValue<int>("Network:Port");
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
            app.MapGrpcService<SystemServiceImpl>();
            app.MapGrpcService<FilesServiceImpl>();
            app.MapGrpcService<ProcessesServiceImpl>();
        });
    })
    .Build();

var cfg = host.Services.GetRequiredService<IConfiguration>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();
var ip = cfg["Network:BindIp"];
var port = cfg.GetValue<int>("Network:Port");
logger.LogInformation("Listening on {ip}:{port} (h2)", ip, port);
if (cfg.GetValue<bool>("Network:EnableUdpQuic"))
    logger.LogInformation("Listening on {ip}:{port} (h3)", ip, port);

await host.RunAsync();
