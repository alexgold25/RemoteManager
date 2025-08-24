using System.IO;
using RemoteManager.Security;
using Xunit;

namespace RM.Tests;

public class TrustedAgentsStoreTests
{
    [Fact]
    public void AddRetrieveAndRemove()
    {
        string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var store = new TrustedAgentsStore(path);

        var rec = new AgentTrustRecord
        {
            DeviceId = "dev1",
            Address = "https://1.2.3.4",
            FingerprintSha256 = "AA:BB",
            Hostname = "host",
            Tags = new[] { "prod" },
            ApprovedBy = "admin"
        };

        store.AddOrUpdate(rec);
        Assert.True(store.ContainsFingerprint("AA:BB"));
        Assert.True(store.TryGetByAddress("https://1.2.3.4", out var found));
        Assert.Equal("dev1", found!.DeviceId);

        // reload
        var store2 = new TrustedAgentsStore(path);
        Assert.True(store2.ContainsFingerprint("AA:BB"));
        store2.Remove("https://1.2.3.4");
        Assert.False(store2.ContainsFingerprint("AA:BB"));
    }
}

