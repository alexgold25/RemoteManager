using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace RemoteManager.ViewModels;

public partial class NavItemVm : ObservableObject
{
    public string TitleKey { get; init; } = string.Empty;
    public string IconKey { get; init; } = string.Empty;
    public NavKey Key { get; init; }

    [ObservableProperty]
    private int? badgeCount;

    public string Title => Application.Current.TryFindResource(TitleKey) as string ?? TitleKey;
    public string Icon => Application.Current.TryFindResource(IconKey) as string ?? string.Empty;
}
