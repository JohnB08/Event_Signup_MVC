using System.Security.Cryptography;
using System.Text;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;

namespace EventSignupApi.Services;

public class UserDtoService
{
    public User GetNewUser(UserDTO dto)
    {
        return new User()
        {
            UserName = dto.UserName,
            Hash = HashPassword(dto.Password)
        };
    }
    private string HashPassword(string password)
    {
        using(var encrypt = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = encrypt.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
    public UserDTO HashDTOValues(UserDTO dto)
    {
        return new UserDTO()
        {
            UserName = dto.UserName,
            Password = HashPassword(dto.Password)
        };
    }
}
