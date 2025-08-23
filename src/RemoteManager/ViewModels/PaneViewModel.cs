using CommunityToolkit.Mvvm.ComponentModel;

namespace RemoteManager.ViewModels;

public class PaneViewModel : ObservableObject
{
    private string _path = "/";
    public string Path
    {
        get => _path;
        set => SetProperty(ref _path, value);
    }
}
