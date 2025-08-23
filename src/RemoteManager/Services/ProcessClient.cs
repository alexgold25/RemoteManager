using System.Threading.Tasks;
using Grpc.Net.Client;
using RM.Proto;

namespace RemoteManager.Services;

public class ProcessClient : IProcessClient
{
    private readonly ProcessesService.ProcessesServiceClient _client;

    public ProcessClient(GrpcChannel channel)
    {
        _client = new ProcessesService.ProcessesServiceClient(channel);
    }

    public Task<ProcessList> ListAsync()
        => _client.ListAsync(new Empty()).ResponseAsync;
}
