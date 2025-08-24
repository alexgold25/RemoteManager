using System;
using Microsoft.Extensions.DependencyInjection;
using RemoteManager.ViewModels;
using RemoteManager.ViewModels.Agents;

namespace RemoteManager.Services.Navigation;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _services;

    public NavigationService(IServiceProvider services)
    {
        _services = services;
    }

    public object Resolve(NavKey key) => key switch
    {
        NavKey.Agents => _services.GetRequiredService<AgentsViewModel>(),
        NavKey.Inbox => _services.GetRequiredService<InboxViewModel>(),
        NavKey.Commander => _services.GetRequiredService<CommanderViewModel>(),
        NavKey.Settings => _services.GetRequiredService<SettingsViewModel>(),
        _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
    };
}
