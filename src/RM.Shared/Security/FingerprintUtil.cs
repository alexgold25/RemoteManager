using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace RM.Shared.Security;

public static class FingerprintUtil
{
    public static string Sha256Hex(X509Certificate2 cert)
    {
        var hash = cert.GetCertHash(HashAlgorithmName.SHA256);
        return string.Join(":", hash.Select(b => b.ToString("X2")));
    }
}
