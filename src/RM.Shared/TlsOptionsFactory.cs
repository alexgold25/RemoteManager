using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace RM.Shared;

public static class TlsOptionsFactory
{
    public static HttpsConnectionAdapterOptions CreateHttp2(X509Certificate2 cert)
    {
        return new HttpsConnectionAdapterOptions
        {
            ServerCertificate = cert,
            SslProtocols = SslProtocols.Tls13
        };
    }

    public static HttpsConnectionAdapterOptions CreateHttp3(X509Certificate2 cert)
    {
        return new HttpsConnectionAdapterOptions
        {
            ServerCertificate = cert,
            SslProtocols = SslProtocols.Tls13
        };
    }

    public static string FingerprintSha256(X509Certificate2 cert)
    {
        return Convert.ToHexString(cert.GetCertHash(HashAlgorithmName.SHA256));
    }
}
