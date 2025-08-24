using System;

namespace RemoteManager.Models;

public class AgentEndpoint
{
    public string DeviceId { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public string Os { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Fingerprint { get; set; } = string.Empty;
    public bool IsTrusted { get; set; }
    public string[] Caps { get; set; } = Array.Empty<string>();
}
