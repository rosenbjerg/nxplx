using System;
using Crypt = BCrypt.Net.BCrypt;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class PasswordUtils
    {
        private static readonly int WorkFactor;
        
        static PasswordUtils()
        {
            var (workFactor, timeUsed) = DetermineWorkFactor();
            {
                WorkFactor = workFactor;
                Console.WriteLine($"BCrypt workfactor set to {workFactor}, taking {timeUsed}ms");
            }
        }
        
        private static string Pepper(string password)
        {
            return $"Nx{password}Plx";
        }
        
        private static (int, double) DetermineWorkFactor()
        {
            var testValue = "jroif87twhgp34958h03";
            var workFactor = 10;
            double timeUsed;
            
            while ((timeUsed = TimeBCrypt(testValue, workFactor++)) < 300) ;

            return (workFactor, timeUsed);
        }

        private static double TimeBCrypt(string password, int workFactor)
        {
            var started = DateTime.UtcNow;
            var bcrypted = Crypt.HashPassword(Pepper(password), workFactor);
            return (bcrypted.Length + DateTime.UtcNow.Subtract(started).TotalMilliseconds) - bcrypted.Length;
        }

        public static string Hash(string password)
        {
            return Crypt.HashPassword(Pepper(password), WorkFactor);
        }

        public static bool Verify(string password, string passwordHash)
        {
            return Crypt.Verify(Pepper(password), passwordHash);
        }
    }
}