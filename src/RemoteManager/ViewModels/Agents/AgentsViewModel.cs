using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Windows;

namespace RemoteManager.ViewModels.Agents;

public partial class AgentsViewModel : ObservableObject
{
    public ObservableCollection<AgentItemVm> Items { get; } = new();
    public ICollectionView View { get; }

    [ObservableProperty]
    private AgentItemVm? selectedItem;

    [ObservableProperty]
    private bool isDetailsOpen;

    [ObservableProperty]
    private AgentStatus activeStatusFilter = AgentStatus.All;

    private string searchQuery = string.Empty;
    public string SearchQuery
    {
        get => searchQuery;
        set
        {
            if (SetProperty(ref searchQuery, value))
            {
                DebounceFilter();
            }
        }
    }

    private CancellationTokenSource? searchDebounceCts;

    public AgentsViewModel()
    {
        View = CollectionViewSource.GetDefaultView(Items);
        View.Filter = Filter;
        Seed();
    }

    private void Seed()
    {
        Items.Add(new AgentItemVm { Status = AgentStatus.Online, Name = "Alpha", Host = "alpha.local", Tags = "prod", Version = "1.0", Uptime = "1d", Platform = "Windows", CpuRam = "10%/2GB", Ping = "10ms", LastSeen = "now", Fingerprint = "AA:BB" });
        Items.Add(new AgentItemVm { Status = AgentStatus.Online, Name = "Beta", Host = "beta.local", Tags = "staging", Version = "1.0", Uptime = "2d", Platform = "Linux", CpuRam = "20%/1GB", Ping = "20ms", LastSeen = "1m", Fingerprint = "CC:DD" });
        Items.Add(new AgentItemVm { Status = AgentStatus.Offline, Name = "Gamma", Host = "gamma.local", Tags = "prod", Version = "1.0", Uptime = "5h", Platform = "Windows", CpuRam = "15%/3GB", Ping = "-", LastSeen = "1h", Fingerprint = "EE:FF" });
        Items.Add(new AgentItemVm { Status = AgentStatus.Pending, Name = "Delta", Host = "delta.local", Tags = "test", Version = "1.0", Uptime = "3h", Platform = "Linux", CpuRam = "5%/512MB", Ping = "15ms", LastSeen = "now", Fingerprint = "GG:HH" });
        Items.Add(new AgentItemVm { Status = AgentStatus.Quarantine, Name = "Epsilon", Host = "eps.local", Tags = "quarantine", Version = "1.0", Uptime = "1h", Platform = "Windows", CpuRam = "50%/4GB", Ping = "-", LastSeen = "5m", Fingerprint = "II:JJ" });
    }

    private void DebounceFilter()
    {
        searchDebounceCts?.Cancel();
        var cts = searchDebounceCts = new CancellationTokenSource();
        Task.Delay(300, cts.Token).ContinueWith(t =>
        {
            if (!t.IsCanceled)
            {
                Application.Current.Dispatcher.Invoke(() => View.Refresh());
            }
        });
    }

    private bool Filter(object obj)
    {
        if (obj is not AgentItemVm agent) return false;
        if (activeStatusFilter != AgentStatus.All && agent.Status != activeStatusFilter)
            return false;
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var q = searchQuery.ToLowerInvariant();
            if (!(agent.Name.ToLowerInvariant().Contains(q) ||
                  agent.Host.ToLowerInvariant().Contains(q) ||
                  agent.Tags.ToLowerInvariant().Contains(q) ||
                  agent.Fingerprint.ToLowerInvariant().Contains(q)))
                return false;
        }
        return true;
    }

    partial void OnActiveStatusFilterChanged(AgentStatus value)
    {
        View.Refresh();
    }

    public IAsyncRelayCommand<AgentItemVm> OpenDetailsCmd => new AsyncRelayCommand<AgentItemVm>(async agent =>
    {
        SelectedItem = agent;
        IsDetailsOpen = true;
        await Task.CompletedTask;
    });

    public IAsyncRelayCommand<AgentItemVm> TrustCmd => new AsyncRelayCommand<AgentItemVm>(_ => Task.CompletedTask);
    public IAsyncRelayCommand<AgentItemVm> RejectCmd => new AsyncRelayCommand<AgentItemVm>(_ => Task.CompletedTask);
    public IAsyncRelayCommand<AgentItemVm> OpenFilesCmd => new AsyncRelayCommand<AgentItemVm>(_ => Task.CompletedTask);
    public IAsyncRelayCommand<AgentItemVm> OpenTerminalCmd => new AsyncRelayCommand<AgentItemVm>(_ => Task.CompletedTask);
    public IAsyncRelayCommand<AgentItemVm> RestartCmd => new AsyncRelayCommand<AgentItemVm>(_ => Task.CompletedTask);

    public IRelayCommand CloseDetailsCmd => new RelayCommand(() => IsDetailsOpen = false);
}
