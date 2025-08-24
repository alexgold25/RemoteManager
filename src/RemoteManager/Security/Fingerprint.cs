using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace RemoteManager.Security;

public static class Fingerprint
{
    public static string ComputeSha256(X509Certificate2 cert)
    {
        var hash = cert.GetCertHash(HashAlgorithmName.SHA256);
        return string.Join(":", hash.Select(b => b.ToString("X2")));
    }
}
