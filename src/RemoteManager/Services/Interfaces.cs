using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Grpc.Net.Client;
using RemoteManager.Models;
using RM.Proto;

namespace RemoteManager.Services;

public interface IDiscoveryService
{
    Task DiscoverAsync();
}

public interface IAgentRegistry
{
    ObservableCollection<AgentEndpoint> Agents { get; }
    void AddOrUpdate(AgentEndpoint agent);
}

public interface IGrpcConnectionFactory
{
    GrpcChannel Create(string address);
}

public interface IFileClient
{
    Task<DirList> ListDirAsync(string path);
}

public interface IProcessClient
{
    Task<ProcessList> ListAsync();
}

public interface ISystemClient
{
    Task<SystemInfo> GetInfoAsync();
}
