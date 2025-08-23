using System.Runtime.InteropServices;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using RM.Proto;

namespace RMAgent.Services;

public class SystemServiceImpl : SystemService.SystemServiceBase
{
    public override Task<SystemInfo> GetInfo(Empty request, ServerCallContext context)
    {
        var info = new SystemInfo
        {
            Os = Environment.OSVersion.ToString(),
            Arch = RuntimeInformation.OSArchitecture.ToString(),
            Version = "0.1",
            DeviceId = Environment.MachineName,
        };
        info.Caps.Add("stub");
        return Task.FromResult(info);
    }

    public override Task<PingReply> Ping(PingRequest request, ServerCallContext context)
    {
        var reply = new PingReply { Ts = request.Ts, EchoTs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
        return Task.FromResult(reply);
    }

    public override Task<UptimeReply> Uptime(Empty request, ServerCallContext context)
    {
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
        return Task.FromResult(new UptimeReply { Seconds = (long)uptime.TotalSeconds });
    }
}
