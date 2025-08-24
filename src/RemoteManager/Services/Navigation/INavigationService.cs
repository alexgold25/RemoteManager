namespace RemoteManager.Services.Navigation;

using RemoteManager.ViewModels;

public interface INavigationService
{
    object Resolve(NavKey key);
}
