using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;

namespace NxPlx.Application.Services
{
    public static class TokenGenerator
    {
        public static string Generate(int length = 18)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            
            return WebEncoders.Base64UrlEncode(bytes);
        }
    }
}