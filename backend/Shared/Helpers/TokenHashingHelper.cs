using System.Security.Cryptography;
using System.Text;

namespace Shared.Helpers;

public class TokenHashingHelper
{
    private static string HashToken(string token)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hashBytes);
    }
}
