using System.Threading.Tasks;
using RemoteManager.Models;
using RM.Shared;
using RM.Proto;

namespace RemoteManager.Services;

public class DiscoveryService : IDiscoveryService
{
    private readonly IAgentRegistry _registry;
    private const string Psk = "dev-psk-please-change";

    public DiscoveryService(IAgentRegistry registry)
    {
        _registry = registry;
    }

    public Task DiscoverAsync()
    {
        // Placeholder: simulate discovery
        _registry.AddOrUpdate(new AgentEndpoint { DeviceId = "dev1", Hostname = "localhost", Os = "stub" });
        return Task.CompletedTask;
    }
}
