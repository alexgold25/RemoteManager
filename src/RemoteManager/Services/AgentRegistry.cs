using System.Collections.ObjectModel;
using System.Linq;
using RemoteManager.Models;
using RemoteManager.Security;

namespace RemoteManager.Services;

public class AgentRegistry : IAgentRegistry
{
    private readonly TrustedAgentsStore _store;
    public ObservableCollection<AgentEndpoint> Agents { get; } = new();

    public AgentRegistry(TrustedAgentsStore store)
    {
        _store = store;
    }

    public void AddOrUpdate(AgentEndpoint agent)
    {
        agent.IsTrusted = _store.ContainsFingerprint(agent.Fingerprint);
        var existing = Agents.FirstOrDefault(a => a.Address == agent.Address);
        if (existing != null)
        {
            var idx = Agents.IndexOf(existing);
            Agents[idx] = agent;
        }
        else
        {
            Agents.Add(agent);
        }
    }
}
