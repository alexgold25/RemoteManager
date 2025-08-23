using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RemoteManager.Models;
using RemoteManager.Services;
using System.Collections.ObjectModel;

namespace RemoteManager.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    private readonly IDiscoveryService _discovery;
    private readonly IAgentRegistry _registry;
    private readonly IFileClient _files;
    private readonly IProcessClient _procs;

    public ObservableCollection<AgentEndpoint> Agents => _registry.Agents;

    public IAsyncRelayCommand DiscoverCommand { get; }
    public IAsyncRelayCommand ListDirCommand { get; }
    public IAsyncRelayCommand ListProcessesCommand { get; }

    public ShellViewModel(IDiscoveryService discovery, IAgentRegistry registry, IFileClient files, IProcessClient procs)
    {
        _discovery = discovery;
        _registry = registry;
        _files = files;
        _procs = procs;
        DiscoverCommand = new AsyncRelayCommand(_discovery.DiscoverAsync);
        ListDirCommand = new AsyncRelayCommand(() => _files.ListDirAsync("/"));
        ListProcessesCommand = new AsyncRelayCommand(() => _procs.ListAsync());
    }
}
