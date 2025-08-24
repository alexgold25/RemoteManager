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
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) =>
        {
            if (cert == null) return false;
            string fp = Fingerprint.ComputeSha256(cert);
            if (_store.ContainsFingerprint(fp)) return true;
            return !_cfg.GetValue<bool>("Security:PinningRequired");
        };
        var httpClient = new HttpClient(handler);
        httpClient.DefaultRequestVersion = new Version(2, 0);
        httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
        return GrpcChannel.ForAddress(address, new GrpcChannelOptions { HttpClient = httpClient });
    }
}
