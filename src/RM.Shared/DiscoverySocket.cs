using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace RM.Shared;

public class DiscoverySocket : IDisposable
{
    private readonly UdpClient _client;

    public DiscoverySocket(int port)
    {
        _client = new UdpClient(port)
        {
            EnableBroadcast = true
        };
    }

    public Task SendAsync(byte[] data, IPEndPoint endpoint, CancellationToken ct)
        => _client.SendAsync(data, data.Length, endpoint, ct);

    public async IAsyncEnumerable<(IPEndPoint, byte[])> ReceiveAsync([EnumeratorCancellation] CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            UdpReceiveResult result = await _client.ReceiveAsync(ct);
            yield return (result.RemoteEndPoint, result.Buffer);
        }
    }

    public void Dispose() => _client.Dispose();
}
