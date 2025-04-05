using System.Security.Cryptography;

namespace AuthService.Services;

public interface IHasherService
{
    public Task<string> GetHash(string password);
}

public class HasherService(IConfiguration _configuration) : IHasherService
{
    
    public async Task<string> GetHash(string token)
    {
        var salt = _configuration.GetSection("HashConfig:Salt");
        return await Task.Run(() =>
        {
            byte[] saltBytes = Convert.FromBase64String(salt.Value);
            
            using var pbkdf2 = new Rfc2898DeriveBytes(token, saltBytes, 100_000, HashAlgorithmName.SHA256);
            byte[] computedHashBytes = pbkdf2.GetBytes(64);
            
            return Convert.ToBase64String(computedHashBytes);
        });
    }
}