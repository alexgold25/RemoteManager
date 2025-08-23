using System;
using Microsoft.Extensions.Configuration;

namespace RM.Shared;

public static class AppEnvironment
{
    public static bool IsDevelopment(IConfiguration cfg)
    {
        var env = cfg["DOTNET_ENVIRONMENT"] ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        return string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase);
    }
}
