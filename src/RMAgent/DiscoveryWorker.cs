using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RM.Shared;
using RM.Proto;

namespace RMAgent;

public class DiscoveryWorker : BackgroundService
{
    private readonly DiscoverySocket _socket;
    private readonly ILogger<DiscoveryWorker> _logger;
    private const string Psk = "dev-psk-please-change";

    public DiscoveryWorker(DiscoverySocket socket, ILogger<DiscoveryWorker> logger)
    {
        _socket = socket;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cert = DevCertLoader.Load();
        var hello = BuildMessage(Discovery.Types.MsgType.HELLO, cert);
        var bytes = DiscoverySerializer.Serialize(hello, Psk);
        await _socket.SendAsync(bytes, new IPEndPoint(IPAddress.Broadcast, 8443), stoppingToken);

        await foreach (var (ep, data) in _socket.ReceiveAsync(stoppingToken))
        {
            if (!DiscoverySerializer.TryDeserialize(data, Psk, out var msg))
                continue;
            if (msg.Type == Discovery.Types.MsgType.DISCOVER)
            {
                var offer = BuildMessage(Discovery.Types.MsgType.OFFER, cert);
                var offerBytes = DiscoverySerializer.Serialize(offer, Psk);
                await _socket.SendAsync(offerBytes, ep, stoppingToken);
            }
        }
    }

    private static Discovery BuildMessage(Discovery.Types.MsgType type, System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        => new()
        {
            Type = type,
            Version = "0.1",
            DeviceId = Environment.MachineName,
            Hostname = Environment.MachineName,
            Os = Environment.OSVersion.ToString(),
            Arch = RuntimeInformation.OSArchitecture.ToString(),
            Endpoints = new Endpoints { H2 = true, Port = 8443 },
            CertFingerprintSha256 = TlsOptionsFactory.FingerprintSha256(cert)
        };
}
