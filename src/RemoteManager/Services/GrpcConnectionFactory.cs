using Grpc.Net.Client;

namespace RemoteManager.Services;

public class GrpcConnectionFactory : IGrpcConnectionFactory
{
    public GrpcChannel Create(string address)
        => GrpcChannel.ForAddress(address);
}
