using System.Text;
using Google.Protobuf;
using RM.Proto;

namespace RM.Shared;

public static class DiscoverySerializer
{
    public const string Magic = "RM01";

    public static byte[] Serialize(Discovery message, string psk)
    {
        var payload = message.ToByteArray();
        var prefix = Encoding.ASCII.GetBytes(Magic);
        var data = prefix.Concat(payload).ToArray();
        var hmac = HmacHelper.ComputeHmac(psk, data);
        return data.Concat(hmac).ToArray();
    }

    public static bool TryDeserialize(ReadOnlySpan<byte> bytes, string psk, out Discovery? message)
    {
        message = null;
        if (bytes.Length < 4 + 32) return false;
        if (!bytes[..4].SequenceEqual(Encoding.ASCII.GetBytes(Magic))) return false;
        var payload = bytes[4..^32].ToArray();
        var sentHmac = bytes[^32..].ToArray();
        var expected = HmacHelper.ComputeHmac(psk, bytes[..(bytes.Length - 32)].ToArray());
        if (!sentHmac.SequenceEqual(expected)) return false;
        message = Discovery.Parser.ParseFrom(payload);
        return true;
    }
}
