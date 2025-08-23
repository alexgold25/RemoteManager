using System.IO;
using Grpc.Core;
using RM.Proto;

namespace RMAgent.Services;

public class FilesServiceImpl : FilesService.FilesServiceBase
{
    public override Task<DirList> ListDir(Path request, ServerCallContext context)
    {
        var list = new DirList();
        try
        {
            foreach (var entry in Directory.EnumerateFileSystemEntries(request.Path))
            {
                var info = new FileInfo(entry);
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

    public override async Task Download(Path request, IServerStreamWriter<FileChunk> responseStream, ServerCallContext context)
    {
        if (File.Exists(request.Path))
        {
            var data = await File.ReadAllBytesAsync(request.Path, context.CancellationToken);
            await responseStream.WriteAsync(new FileChunk { Path = request.Path, Data = Google.Protobuf.ByteString.CopyFrom(data) });
        }
    }

    public override Task<OpStatus> Upload(IAsyncStreamReader<FileChunk> requestStream, ServerCallContext context)
        => Task.FromResult(new OpStatus { Ok = true });
}
