using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace RemoteManager.ViewModels.Agents;

public enum AgentStatus
{
    All,
    Online,
    Offline,
    Pending,
    Quarantine
}

public class AgentItemVm : ObservableObject
{
    public AgentStatus Status { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Uptime { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string CpuRam { get; set; } = string.Empty;
    public string Ping { get; set; } = string.Empty;
    public string LastSeen { get; set; } = string.Empty;
    public string Fingerprint { get; set; } = string.Empty;
    public Guid DeviceId { get; set; } = Guid.NewGuid();
}
