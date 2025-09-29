using System.Text;
using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;

namespace CSharp_BookStop.API.Services;

public class AuthService : IAuthService
{
    public string HashPassword(string password, byte[] salt)
    {
        var config = new Argon2Config
        {
            Type = Argon2Type.DataIndependentAddressing,
            Version = Argon2Version.Nineteen,
            TimeCost = 10,
            MemoryCost = 65536,
            Lanes = Environment.ProcessorCount,
            Password = Encoding.UTF8.GetBytes(password),
            Salt = salt,
            HashLength = 64
        };
        var argon2 = new Argon2(config);
        using var hashA = argon2.Hash();
        var hashString = config.EncodeString(hashA.Buffer);

        return hashString;
    }

    public bool VerifyPassword(string password, byte[] salt, string hash)
    {
        var matchingPasswords = false;
        var config = new Argon2Config
        {
            Type = Argon2Type.DataIndependentAddressing,
            Version = Argon2Version.Nineteen,
            TimeCost = 10,
            MemoryCost = 65536,
            Lanes = Environment.ProcessorCount,
            Password = Encoding.UTF8.GetBytes(password),
            Salt = salt,
            HashLength = 64
        };
        SecureArray<byte>? hashB = null;
        try
        {
            if (config.DecodeString(hash, out hashB) && hashB != null)
            {
                var argon2 = new Argon2(config);
                using var hashToVerify = argon2.Hash();
                if (Argon2.FixedTimeEquals(hashB, hashToVerify))
                {
                    matchingPasswords = true;
                }
            }
        }
        finally
        {
            hashB?.Dispose();
        }
        return matchingPasswords;
    }
}