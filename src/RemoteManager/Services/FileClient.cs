using System.Threading.Tasks;
using Grpc.Net.Client;
using RM.Proto;

namespace RemoteManager.Services
{
    public class FileClient : IFileClient
    {
        private readonly FilesService.FilesServiceClient _client;

        public FileClient(GrpcChannel channel)
        {
            _client = new FilesService.FilesServiceClient(channel);
        }

        public Task<DirList> ListDirAsync(string path)
        {
            return _client.ListDirAsync(new Path { Path_ = path }).ResponseAsync;
        }
    }
}
