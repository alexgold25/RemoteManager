using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

namespace RM.Shared
{
    public static class DevCertLoader
    {
        public static X509Certificate2 Load(IConfiguration cfg)
        {
            string path = cfg["Security:DevCertPath"] ?? throw new FileNotFoundException("Security:DevCertPath not set");
            string? password = cfg["Security:DevCertPassword"];
            string fullPath = Path.Combine(AppContext.BaseDirectory, path);
            return new X509Certificate2(fullPath, password, X509KeyStorageFlags.Exportable);
        }
    }
}
