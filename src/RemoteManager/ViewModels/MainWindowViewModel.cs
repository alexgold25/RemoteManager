using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RemoteManager.Services;
using RemoteManager.Services.Navigation;
using RemoteManager.ViewModels.Agents;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RemoteManager.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly IDiscoveryService _discovery;

    public ObservableCollection<NavItemVm> NavItems { get; } = new();

    [ObservableProperty]
    private NavItemVm? selectedNavItem;

    [ObservableProperty]
    private object? currentViewModel;

    [ObservableProperty]
    private int inboxCount;

    [ObservableProperty]
    private int onlineCount;

    [ObservableProperty]
    private int offlineCount;

    [ObservableProperty]
    private int pendingCount;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private string statusText = string.Empty;

    [ObservableProperty]
    private string currentTitle = string.Empty;

    [ObservableProperty]
    private string inboxBannerText = string.Empty;

    public IRelayCommand<NavItemVm> NavigateCommand { get; }
    public IAsyncRelayCommand ScanNetworkCommand { get; }

    public MainWindowViewModel(INavigationService navigation, IDiscoveryService discovery)
    {
        _navigation = navigation;
        _discovery = discovery;

        NavItems.Add(new NavItemVm { TitleKey = "Nav.Agents", IconKey = "Icon.Agents", Key = NavKey.Agents });
        NavItems.Add(new NavItemVm { TitleKey = "Nav.Inbox", IconKey = "Icon.Inbox", Key = NavKey.Inbox });
        NavItems.Add(new NavItemVm { TitleKey = "Nav.Commander", IconKey = "Icon.Commander", Key = NavKey.Commander });
        NavItems.Add(new NavItemVm { TitleKey = "Nav.Settings", IconKey = "Icon.Settings", Key = NavKey.Settings });

        NavigateCommand = new RelayCommand<NavItemVm>(Navigate);
        ScanNetworkCommand = new AsyncRelayCommand(_discovery.DiscoverAsync);

        SelectedNavItem = NavItems.First();
        Navigate(SelectedNavItem);
        OnInboxCountChanged(InboxCount);
    }

    private void Navigate(NavItemVm? item)
    {
        if (item == null) return;
        SelectedNavItem = item;
        CurrentViewModel = _navigation.Resolve(item.Key);
        CurrentTitle = item.Title;
        if (CurrentViewModel is AgentsViewModel avm)
        {
            avm.SearchQuery = SearchQuery;
            UpdateCountsFromAgents(avm);
        }
    }

    partial void OnInboxCountChanged(int value)
    {
        var inboxItem = NavItems.FirstOrDefault(n => n.Key == NavKey.Inbox);
        if (inboxItem != null)
            inboxItem.BadgeCount = value > 0 ? value : null;
        var template = Application.Current.TryFindResource("Banner.InboxFound") as string ?? "Found new devices: {0} — View";
        InboxBannerText = string.Format(template, value);
    }

    partial void OnSearchQueryChanged(string value)
    {
        if (CurrentViewModel is AgentsViewModel avm)
            avm.SearchQuery = value;
    }

    partial void OnOnlineCountChanged(int value) => UpdateStatus();
    partial void OnOfflineCountChanged(int value) => UpdateStatus();
    partial void OnPendingCountChanged(int value) => UpdateStatus();

    private void UpdateStatus()
    {
        string online = Application.Current.TryFindResource("StatusBar.Online") as string ?? "Online";
        string offline = Application.Current.TryFindResource("StatusBar.Offline") as string ?? "Offline";
        string pending = Application.Current.TryFindResource("StatusBar.Pending") as string ?? "Pending";
        string last = Application.Current.TryFindResource("StatusBar.LastContact") as string ?? "Last contact";
        StatusText = $"{online}: {OnlineCount} | {offline}: {OfflineCount} | {pending}: {PendingCount} | {last}: -";
    }

    private void UpdateCountsFromAgents(AgentsViewModel vm)
    {
        OnlineCount = vm.Items.Count(i => i.Status == AgentStatus.Online);
        OfflineCount = vm.Items.Count(i => i.Status == AgentStatus.Offline);
        PendingCount = vm.Items.Count(i => i.Status == AgentStatus.Pending);
    }
}
