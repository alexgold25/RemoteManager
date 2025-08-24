using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using RemoteManager.Security;
using Xunit;

namespace RM.Tests;

public class FingerprintTests
{
    [Fact]
    public void ComputeSha256_FormatsWithColons()
    {
        using var rsa = RSA.Create(2048);
        var req = new CertificateRequest("CN=Test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var cert = req.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));

        string fp1 = Fingerprint.ComputeSha256(cert);
        string fp2 = Fingerprint.ComputeSha256(cert);

        Assert.Equal(fp1, fp2); // deterministic
        Assert.Matches("^([0-9A-F]{2}:){31}[0-9A-F]{2}$", fp1);
    }
}

