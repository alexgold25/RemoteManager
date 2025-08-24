using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RM.Shared.Security;
using RM.Proto;

namespace RMAgent;

public class DiscoveryWorker : BackgroundService
{
    private readonly DiscoverySocket _socket;
    private readonly ILogger<DiscoveryWorker> _logger;
    private readonly IConfiguration _config;
    private readonly string _deviceId;

    public DiscoveryWorker(DiscoverySocket socket, ILogger<DiscoveryWorker> logger, IConfiguration config)
    {
        _socket = socket;
        _logger = logger;
        _config = config;
        _deviceId = DeviceIdentity.EnsureDeviceId(Path.Combine(AppContext.BaseDirectory, "device.id"));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cert = CertManager.EnsureServerCertificate(_config, "Agent");
        var psk = _config["Security:DiscoveryPsk"] ?? string.Empty;
        var port = _config.GetValue<int>("Network:Port");
        var hello = BuildMessage(Discovery.Types.MsgType.Hello, _deviceId, cert, port);
        var bytes = DiscoverySerializer.Serialize(hello, psk);
        await _socket.SendAsync(bytes, new IPEndPoint(IPAddress.Broadcast, port), stoppingToken);

        await foreach (var (ep, data) in _socket.ReceiveAsync(stoppingToken))
        {
            if (!DiscoverySerializer.TryDeserialize(data, psk, out var msg))
                continue;
            if (msg.Type == Discovery.Types.MsgType.Discover)
            {
                var offer = BuildMessage(Discovery.Types.MsgType.Offer, _deviceId, cert, port);
                var offerBytes = DiscoverySerializer.Serialize(offer, psk);
                await _socket.SendAsync(offerBytes, ep, stoppingToken);
            }
        }
    }

    private static Discovery BuildMessage(Discovery.Types.MsgType type, string deviceId, X509Certificate2 cert, int port)
        => new()
        {
            Type = type,
            Version = "0.1",
            DeviceId = deviceId,
            Hostname = Environment.MachineName,
            Os = Environment.OSVersion.ToString(),
            Arch = RuntimeInformation.OSArchitecture.ToString(),
            Endpoints = new Endpoints { H2 = true, Port = port },
            CertFingerprintSha256 = FingerprintUtil.Sha256Hex(cert)
        };
}
