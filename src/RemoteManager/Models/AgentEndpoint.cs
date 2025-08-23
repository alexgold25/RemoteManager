using System;

namespace RemoteManager.Models;

public class AgentEndpoint
{
    public string DeviceId { get; set; } = "";
    public string Hostname { get; set; } = "";
    public string Os { get; set; } = "";
    public string[] Caps { get; set; } = Array.Empty<string>();
}
