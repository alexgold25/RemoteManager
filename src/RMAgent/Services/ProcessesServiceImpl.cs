using System.Diagnostics;
using Grpc.Core;
using RM.Proto;

namespace RMAgent.Services;

public class ProcessesServiceImpl : ProcessesService.ProcessesServiceBase
{
    public override Task<ProcessList> List(Empty request, ServerCallContext context)
    {
        var list = new ProcessList();
        foreach (var p in Process.GetProcesses())
        {
            list.Items.Add(new ProcessInfo
            {
                Pid = p.Id,
                Name = p.ProcessName,
                User = string.Empty,
                Cpu = 0,
                RamBytes = p.WorkingSet64
            });
        }
        return Task.FromResult(list);
    }

    public override Task<OpStatus> Kill(Pid request, ServerCallContext context)
    {
        try
        {
            var proc = Process.GetProcessById(request.Pid);
            proc.Kill();
            return Task.FromResult(new OpStatus { Ok = true });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new OpStatus { Ok = false, Error = ex.Message });
        }
    }

    public override Task<StartReply> Start(StartRequest request, ServerCallContext context)
    {
        try
        {
            var proc = Process.Start(new ProcessStartInfo(request.Path, request.Args ?? string.Empty)
            {
                WorkingDirectory = string.IsNullOrEmpty(request.Wd) ? null : request.Wd
            });
            return Task.FromResult(new StartReply { Ok = true, Pid = proc?.Id ?? 0 });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new StartReply { Ok = false, Error = ex.Message });
        }
    }
}
