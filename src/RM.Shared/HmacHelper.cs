using System.Security.Cryptography;
using System.Text;

namespace RM.Shared;

public static class HmacHelper
{
    public static byte[] ComputeHmac(string psk, byte[] data)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(psk));
        return hmac.ComputeHash(data);
    }
}
