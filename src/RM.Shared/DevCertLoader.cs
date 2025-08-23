using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

namespace RM.Shared;

public static class DevCertLoader
{
    public static X509Certificate2 Load(IConfiguration cfg)
    {
        var path = cfg["Security:DevCertPath"] ?? throw new FileNotFoundException("Security:DevCertPath not set");
        var password = cfg["Security:DevCertPassword"];
        var fullPath = Path.Combine(AppContext.BaseDirectory, path);
        return new X509Certificate2(fullPath, password, X509KeyStorageFlags.Exportable);
    }
}
