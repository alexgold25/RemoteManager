using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace RM.Shared;

public static class DevCertLoader
{
    public static X509Certificate2 Load()
    {
        var certDir = Path.Combine(AppContext.BaseDirectory, "..", "certificates", "dev");
        if (!Directory.Exists(certDir))
            throw new DirectoryNotFoundException(certDir);
        var file = Directory.GetFiles(certDir, "*.pfx").FirstOrDefault()
                   ?? throw new FileNotFoundException("No dev certificate", certDir);
        return new X509Certificate2(file, "dev", X509KeyStorageFlags.Exportable);
    }
}
