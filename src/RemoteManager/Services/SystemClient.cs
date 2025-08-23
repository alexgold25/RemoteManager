using System.Threading.Tasks;
using Grpc.Net.Client;
using RM.Proto;

namespace RemoteManager.Services;

public class SystemClient : ISystemClient
{
    private readonly SystemService.SystemServiceClient _client;

    public SystemClient(GrpcChannel channel)
    {
        _client = new SystemService.SystemServiceClient(channel);
    }

    public Task<SystemInfo> GetInfoAsync()
        => _client.GetInfoAsync(new Empty()).ResponseAsync;
}
