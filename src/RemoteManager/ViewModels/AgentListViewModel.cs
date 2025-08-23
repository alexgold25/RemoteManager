using CommunityToolkit.Mvvm.ComponentModel;
using RemoteManager.Models;
using RemoteManager.Services;
using System.Collections.ObjectModel;

namespace RemoteManager.ViewModels;

public class AgentListViewModel : ObservableObject
{
    private readonly IAgentRegistry _registry;
    public ObservableCollection<AgentEndpoint> Agents => _registry.Agents;

    public AgentListViewModel(IAgentRegistry registry)
    {
        _registry = registry;
    }
}
