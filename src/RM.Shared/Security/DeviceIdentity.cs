using System;
using System.IO;

namespace RM.Shared.Security;

public static class DeviceIdentity
{
    public static string EnsureDeviceId(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        if (File.Exists(path))
            return File.ReadAllText(path).Trim();

        var id = Guid.NewGuid().ToString();
        File.WriteAllText(path, id);
        return id;
    }
}
