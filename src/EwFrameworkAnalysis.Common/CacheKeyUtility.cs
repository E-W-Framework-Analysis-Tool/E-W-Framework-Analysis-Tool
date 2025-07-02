using System.Security.Cryptography;
using System.Text;

namespace EwFrameworkAnalysis.Common;

public class CacheKeyUtility
{
    public static string CalculateHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}
