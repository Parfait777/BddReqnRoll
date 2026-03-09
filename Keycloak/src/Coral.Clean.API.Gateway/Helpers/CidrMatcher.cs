using System.Net;
using System.Net.Sockets;

namespace Coral.Clean.API.Gateway.Helpers
{
    /// <summary>
    /// Provides CIDR-based IP address matching utilities.
    /// </summary>
    /// <remarks>
    /// Supports IPv4 and IPv6 CIDR matching (including IPv4-mapped IPv6 addresses).
    /// Intended for gateway-level filtering scenarios such as IP whitelisting.
    /// </remarks>
    public static class CidrMatcher
    {
        /// <summary>
        /// Determines whether a given IP address falls within a specified CIDR range.
        /// </summary>
        /// <param name="ip">
        /// The client IP address to evaluate.
        /// </param>
        /// <param name="cidr">
        /// The CIDR notation to test against (e.g. "203.0.113.10/32", "10.0.0.0/8", "::1/128", "2001:db8::/32").
        /// </param>
        /// <returns>
        /// <c>true</c> if the IP address is contained within the CIDR range; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(IPAddress ip, string cidr)
        {
            // Convenience: treat loopback as allowed (useful for dev/test).
            if (IPAddress.IsLoopback(ip))
            {
                return true; // 127.0.0.1 or ::1
            }

            // CIDR notation must be in the form "baseIp/prefixLength".
            string[] parts = cidr.Split('/');
            if (parts.Length != 2)
            {
                return false;
            }

            // Parse the base IP address portion of the CIDR.
            if (!IPAddress.TryParse(parts[0], out IPAddress? baseIp))
            {
                return false;
            }

            // Parse and validate prefix length.
            if (!int.TryParse(parts[1], out int prefix))
            {
                return false;
            }

            // Normalize IPv4-mapped IPv6 addresses (e.g., ::ffff:10.0.0.1) to IPv4.
            if (ip.IsIPv4MappedToIPv6)
            {
                ip = ip.MapToIPv4();
            }

            if (baseIp.IsIPv4MappedToIPv6)
            {
                baseIp = baseIp.MapToIPv4();
            }

            // Address families must match after normalization.
            if (ip.AddressFamily != baseIp.AddressFamily)
            {
                return false;
            }

            return ip.AddressFamily switch
            {
                AddressFamily.InterNetwork => ContainsV4(ip, baseIp, prefix),
                AddressFamily.InterNetworkV6 => ContainsV6(ip, baseIp, prefix),
                _ => false
            };
        }

        private static bool ContainsV4(IPAddress ip, IPAddress baseIp, int prefix)
        {
            // Prefix must be between 0 and 32 for IPv4.
            if (prefix is < 0 or > 32)
            {
                return false;
            }

            byte[] ipBytes = ip.GetAddressBytes();
            byte[] baseBytes = baseIp.GetAddressBytes();

            // Build subnet mask based on the prefix length.
            // prefix = 24 => mask = 255.255.255.0
            // prefix = 32 => mask = 255.255.255.255
            // prefix = 0  => mask = 0.0.0.0 (matches everything)
            uint mask = prefix == 0 ? 0u : uint.MaxValue << (32 - prefix);

            uint ipVal = ToUInt32(ipBytes);
            uint baseVal = ToUInt32(baseBytes);

            return (ipVal & mask) == (baseVal & mask);
        }

        private static bool ContainsV6(IPAddress ip, IPAddress baseIp, int prefix)
        {
            // Prefix must be between 0 and 128 for IPv6.
            if (prefix is < 0 or > 128)
            {
                return false;
            }

            byte[] ipBytes = ip.GetAddressBytes();       // 16 bytes
            byte[] baseBytes = baseIp.GetAddressBytes(); // 16 bytes

            // Compare full bytes covered by the prefix.
            int fullBytes = prefix / 8;
            int remainingBits = prefix % 8;

            for (int i = 0; i < fullBytes; i++)
            {
                if (ipBytes[i] != baseBytes[i])
                {
                    return false;
                }
            }

            // Compare remaining bits, if any.
            if (remainingBits == 0)
            {
                return true; // exact match on all prefix bytes
            }

            // Mask for the high 'remainingBits' bits of the next byte.
            // Example: remainingBits=3 => 1110_0000
            byte mask = (byte)(0xFF << (8 - remainingBits));

            return (ipBytes[fullBytes] & mask) == (baseBytes[fullBytes] & mask);
        }

        /// <summary>
        /// Converts a 4-byte IPv4 address into a 32-bit unsigned integer.
        /// </summary>
        private static uint ToUInt32(byte[] bytes) =>
            ((uint)bytes[0] << 24) |
            ((uint)bytes[1] << 16) |
            ((uint)bytes[2] << 8) |
             bytes[3];
    }
}
