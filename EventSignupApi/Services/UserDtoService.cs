using System.Security.Cryptography;
using System.Text;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;

namespace EventSignupApi.Services;

public class UserDtoService
{
    /// <summary>
    /// Gets a new user connected to a dto, hashes the password using SHA256
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public static User GetNewUser(UserDTO dto)
    {
        return new User()
        {
            UserName = dto.UserName,
            Hash = HashPassword(dto.Password)
        };
    }
    /// <summary>
    /// Method for hashing a password using SHA256
    /// !!!THIS HAS NO SALTING, FOR DEMONSTRATION ONLY
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    private static string HashPassword(string password)
    {
        var salt = Environment.GetEnvironmentVariable("PWSALT");
        var bytes = Encoding.UTF8.GetBytes(password + salt);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
    /// <summary>
    /// Returns a DTO with hashed values. 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public static UserDTO HashDtoValues(UserDTO dto)
    {
        return new UserDTO()
        {
            UserName = dto.UserName,
            Password = HashPassword(dto.Password)
        };
    }
}
