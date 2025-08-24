using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

namespace RM.Shared.Security;

public static class CertManager
{
    public static X509Certificate2 EnsureServerCertificate(IConfiguration cfg, string role)
    {
        var path = cfg["Security:DevCertPath"];
        var pwd = cfg["Security:DevCertPassword"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(path))
            path = Path.Combine(GetDefaultCertDirectory(role), role.ToLower() + ".pfx");

        var fullPath = Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path);
        var dir = Path.GetDirectoryName(fullPath)!;
        Directory.CreateDirectory(dir);

        if (File.Exists(fullPath))
            return new X509Certificate2(fullPath, pwd, X509KeyStorageFlags.Exportable);

        var subject = cfg["Security:Subject"] ?? $"CN={role}";
        var validity = cfg.GetValue("Security:ValidityDays", 825);
        var addHosts = cfg.GetValue("Security:AddHostnamesToSAN", true);
        var addIps = cfg.GetValue("Security:AddIPsToSAN", true);
        var hostnames = addHosts ? CollectHostnames() : Enumerable.Empty<string>();
        var ips = addIps ? CollectIPs() : Enumerable.Empty<IPAddress>();

        var cert = GenerateSelfSigned(subject, hostnames, ips, validity);
        SaveAsPfx(cert, fullPath, pwd);
        return new X509Certificate2(fullPath, pwd, X509KeyStorageFlags.Exportable);
    }

    public static X509Certificate2 GenerateSelfSigned(string subject, IEnumerable<string> hostnames, IEnumerable<IPAddress> ips, int validityDays)
    {
        using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var req = new CertificateRequest(subject, key, HashAlgorithmName.SHA256);
        req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, false));
        req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection
        {
            new Oid("1.3.6.1.5.5.7.3.1"), // Server Auth
            new Oid("1.3.6.1.5.5.7.3.2")  // Client Auth
        }, true));

        if (hostnames.Any() || ips.Any())
        {
            var sanBuilder = new SubjectAlternativeNameBuilder();
            foreach (var h in hostnames.Distinct())
                sanBuilder.AddDnsName(h);
            foreach (var ip in ips.Distinct())
                sanBuilder.AddIpAddress(ip);
            req.CertificateExtensions.Add(sanBuilder.Build());
        }

        var notBefore = DateTimeOffset.UtcNow.AddDays(-1);
        var notAfter = notBefore.AddDays(validityDays);
        return req.CreateSelfSigned(notBefore, notAfter);
    }

    public static void SaveAsPfx(X509Certificate2 cert, string fullPath, string password)
    {
        var dir = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllBytes(fullPath, cert.Export(X509ContentType.Pfx, password));
    }

    public static string GetDefaultCertDirectory(string role)
    {
        if (OperatingSystem.IsWindows())
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "RemoteManager", "certs");
        else
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RemoteManager", "certs");
    }

    public static IEnumerable<string> CollectHostnames()
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { Environment.MachineName };
        try
        {
            var host = Dns.GetHostName();
            names.Add(host);
            var entry = Dns.GetHostEntry(host);
            foreach (var alias in entry.Aliases)
                names.Add(alias);
        }
        catch { }
        return names;
    }

    public static IEnumerable<IPAddress> CollectIPs()
    {
        try
        {
            var host = Dns.GetHostName();
            var entry = Dns.GetHostEntry(host);
            return entry.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6);
        }
        catch
        {
            return Array.Empty<IPAddress>();
        }
    }
}
