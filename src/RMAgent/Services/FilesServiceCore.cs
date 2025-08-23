using System.IO;
using Grpc.Core;
using Google.Protobuf;
using RM.Proto;
using ProtoPath = RM.Proto.Path;

namespace RMAgent.Services
{
    public class FilesServiceCore : FilesService.FilesServiceBase
    {
        public override Task<DirList> ListDir(ProtoPath request, ServerCallContext context)
        {
            DirList list = new DirList();
            try
            {
                foreach (string entry in Directory.EnumerateFileSystemEntries(request.Path))
                {
                    FileInfo info = new FileInfo(entry);
                    list.Entries.Add(new DirEntry
                    {
                        Name = info.Name,
                        IsDir = (info.Attributes & FileAttributes.Directory) != 0,
                        Size = info.Exists ? info.Length : 0,
                        MtimeUnix = new DateTimeOffset(info.LastWriteTimeUtc).ToUnixTimeSeconds()
                    });
                }
            }
            catch
            {
            }

            return Task.FromResult(list);
        }

        public override async Task Download(ProtoPath request, IServerStreamWriter<FileChunk> responseStream, ServerCallContext context)
        {
            if (File.Exists(request.Path))
            {
                byte[] data = await File.ReadAllBytesAsync(request.Path, context.CancellationToken);
                await responseStream.WriteAsync(new FileChunk { Path = request.Path, Data = ByteString.CopyFrom(data) });
            }
        }

        public override Task<OpStatus> Upload(IAsyncStreamReader<FileChunk> requestStream, ServerCallContext context)
        {
            return Task.FromResult(new OpStatus { Ok = true });
        }
    }
}
