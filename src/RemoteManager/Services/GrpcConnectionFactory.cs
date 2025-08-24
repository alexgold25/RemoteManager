using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using RemoteManager.Security;

namespace RemoteManager.Services;

public class GrpcConnectionFactory : IGrpcConnectionFactory
{
    private readonly TrustedAgentsStore _store;
    private readonly IConfiguration _cfg;

    public GrpcConnectionFactory(TrustedAgentsStore store, IConfiguration cfg)
    {
        _store = store;
        _cfg = cfg;
    }

    public GrpcChannel Create(string address)
    {
        var modeStr = _cfg.GetValue<string>("Security:PinningRequired") ?? "Off";
        Enum.TryParse<PinningMode>(modeStr, true, out var mode);

        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) =>
        {
            if (cert == null) return false;
            string fp = Fingerprint.ComputeSha256(cert);
            bool known = _store.ContainsFingerprint(fp);
            return mode switch
            {
                PinningMode.Strict => known,
                PinningMode.Relaxed => true,
                _ => true
            };
        };

        var httpClient = new HttpClient(handler)
        {
            DefaultRequestVersion = new Version(2, 0),
            DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
        };
        return GrpcChannel.ForAddress(address, new GrpcChannelOptions { HttpClient = httpClient });
    }
}

public enum PinningMode
{
    Off,
    Relaxed,
    Strict
}
