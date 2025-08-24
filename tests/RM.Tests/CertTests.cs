using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using RM.Shared.Security;
using Xunit;

namespace RM.Tests;

public class CertTests
{
    [Fact]
    public void EnsureServerCertificate_HasSan()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".pfx");
        var cfgDict = new Dictionary<string, string?>
        {
            ["Security:DevCertPath"] = tempPath,
            ["Security:DevCertPassword"] = string.Empty,
            ["Security:Subject"] = "CN=Test",
            ["Security:AddHostnamesToSAN"] = "true",
            ["Security:AddIPsToSAN"] = "true"
        };
        var cfg = new ConfigurationBuilder().AddInMemoryCollection(cfgDict).Build();
        var cert = CertManager.EnsureServerCertificate(cfg, "Agent");
        var sanExt = cert.Extensions["2.5.29.17"];
        Assert.NotNull(sanExt);
        string san = new AsnEncodedData(sanExt!.Oid, sanExt.RawData).Format(true);
        var host = CertManager.CollectHostnames().First();
        var ip = CertManager.CollectIPs().First();
        Assert.Contains(host, san, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(ip.ToString(), san);
    }
}
