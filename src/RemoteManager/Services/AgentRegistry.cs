using System.Collections.ObjectModel;
using RemoteManager.Models;

namespace RemoteManager.Services;

public class AgentRegistry : IAgentRegistry
{
    public ObservableCollection<AgentEndpoint> Agents { get; } = new();

    public void AddOrUpdate(AgentEndpoint agent)
    {
        Agents.Add(agent);
    }
}
