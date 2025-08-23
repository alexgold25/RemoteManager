using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RM.Shared;
using RM.Proto;

namespace RMAgent;

public class DiscoveryWorker : BackgroundService
{
    private readonly DiscoverySocket _socket;
    private readonly ILogger<DiscoveryWorker> _logger;
    private readonly IConfiguration _config;

    public DiscoveryWorker(DiscoverySocket socket, ILogger<DiscoveryWorker> logger, IConfiguration config)
    {
        _socket = socket;
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cert = DevCertLoader.Load(_config);
        var psk = _config["Security:DiscoveryPsk"] ?? string.Empty;
        var port = _config.GetValue<int>("Network:Port");
        var hello = BuildMessage(Discovery.Types.MsgType.HELLO, cert, port);
        var bytes = DiscoverySerializer.Serialize(hello, psk);
        await _socket.SendAsync(bytes, new IPEndPoint(IPAddress.Broadcast, port), stoppingToken);

        await foreach (var (ep, data) in _socket.ReceiveAsync(stoppingToken))
        {
            if (!DiscoverySerializer.TryDeserialize(data, psk, out var msg))
                continue;
            if (msg.Type == Discovery.Types.MsgType.DISCOVER)
            {
                var offer = BuildMessage(Discovery.Types.MsgType.OFFER, cert, port);
                var offerBytes = DiscoverySerializer.Serialize(offer, psk);
                await _socket.SendAsync(offerBytes, ep, stoppingToken);
            }
        }
    }

    private static Discovery BuildMessage(Discovery.Types.MsgType type, System.Security.Cryptography.X509Certificates.X509Certificate2 cert, int port)
        => new()
        {
            Type = type,
            Version = "0.1",
            DeviceId = Environment.MachineName,
            Hostname = Environment.MachineName,
            Os = Environment.OSVersion.ToString(),
            Arch = RuntimeInformation.OSArchitecture.ToString(),
            Endpoints = new Endpoints { H2 = true, Port = port },
            CertFingerprintSha256 = TlsOptionsFactory.FingerprintSha256(cert)
        };
}
