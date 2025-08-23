using System.Runtime.InteropServices;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using RM.Proto;

namespace RMAgent.Services
{
    public class SystemServiceImpl : SystemService.SystemServiceBase
    {
        public override Task<SystemInfo> GetInfo(Empty request, ServerCallContext context)
        {
            SystemInfo info = new SystemInfo
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
            PingReply reply = new PingReply { Ts = request.Ts, EchoTs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
            return Task.FromResult(reply);
        }

        public override Task<UptimeReply> Uptime(Empty request, ServerCallContext context)
        {
            TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
            return Task.FromResult(new UptimeReply { Seconds = (long)uptime.TotalSeconds });
        }
    }
}
