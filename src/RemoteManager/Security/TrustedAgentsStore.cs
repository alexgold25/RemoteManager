using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RemoteManager.Security;

public class TrustedAgent
{
    public string DeviceId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string FingerprintSha256 { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public DateTime AddedUtc { get; set; } = DateTime.UtcNow;
}

public class TrustedAgentsStore
{
    private readonly string _filePath;
    private readonly List<TrustedAgent> _agents = new();

    public TrustedAgentsStore(string? path = null)
    {
        _filePath = path ?? GetDefaultPath();
        Load();
    }

    private static string GetDefaultPath()
    {
        string baseDir;
        try
        {
            baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RemoteManager");
        }
        catch
        {
            baseDir = Path.Combine(AppContext.BaseDirectory, "data");
        }
        Directory.CreateDirectory(baseDir);
        return Path.Combine(baseDir, "trusted_agents.json");
    }

    public void Load()
    {
        _agents.Clear();
        if (File.Exists(_filePath))
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                var list = JsonSerializer.Deserialize<List<TrustedAgent>>(json);
                if (list != null)
                    _agents.AddRange(list);
            }
            catch { }
        }
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(_agents, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    public IEnumerable<TrustedAgent> List() => _agents;

    public void AddOrUpdate(TrustedAgent agent)
    {
        var existing = _agents.FirstOrDefault(a => a.Address == agent.Address ||
                                                   (!string.IsNullOrEmpty(agent.DeviceId) && a.DeviceId == agent.DeviceId));
        if (existing != null)
        {
            existing.FingerprintSha256 = agent.FingerprintSha256;
            existing.Hostname = agent.Hostname;
            existing.DeviceId = agent.DeviceId;
        }
        else
        {
            agent.AddedUtc = DateTime.UtcNow;
            _agents.Add(agent);
        }
        Save();
    }

    public bool TryGetByAddress(string address, out TrustedAgent? agent)
    {
        agent = _agents.FirstOrDefault(a => a.Address == address);
        return agent != null;
    }

    public bool ContainsFingerprint(string fingerprint)
        => _agents.Any(a => a.FingerprintSha256.Equals(fingerprint, StringComparison.OrdinalIgnoreCase));
}
