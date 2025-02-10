using EventSignupApi.Context;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;
using EventSignupApi.Models.HandlerResult;
using Microsoft.EntityFrameworkCore;

namespace EventSignupApi.Services;

public class UserHandler(DatabaseContext context, UserDtoService dtoService, TokenService tokenService)
{   
    private readonly DatabaseContext _context = context;
    private readonly UserDtoService _dtoService = dtoService;
    
    private readonly TokenService _tokenService = tokenService;

    /// <summary>
    /// Creates a new user based on DTO
    /// Returns a sessionToken as data on success,
    /// Errormessage on failure.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<HandlerResult<string>> CreateNewUser(UserDTO dto)
    {
        if (_context.Users.Any(u => u.UserName == dto.UserName))
            return HandlerResult<string>.Error("Username already taken");
        try
        {
            var newUser = UserDtoService.GetNewUser(dto);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            var token  = _tokenService.CreateSession(newUser.UserName);
            return HandlerResult<string>.Ok(token);
        }
        catch (Exception ex)
        {
            return HandlerResult<string>.Error(ex.Message);
        }
    }
    /// <summary>
    /// Validates a userDTO, checks dto hash vs stored hash.
    /// returns action message as Data/ErrorMessage
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<HandlerResult<string>> ValidateUserDto(UserDTO dto)
    {
        var hashedValues = UserDtoService.HashDtoValues(dto);
        var existingUser = await _context.Users.Where(u => u.Hash == hashedValues.Password).FirstOrDefaultAsync();
        return existingUser == null ? HandlerResult<string>.Error("Missing user") : HandlerResult<string>.Ok("User validated");
    }

    /// <summary>
    /// Gets a user based on username. 
    /// Private method used after validating a token.
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    private async Task<HandlerResult<User>> GetUser(string userName)
    {
        var user = await _context.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
        return user == null ? HandlerResult<User>.Error("Missing user") : HandlerResult<User>.Ok(user);
    }
    /// <summary>
    /// Creates a session using a UserName
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public string CreateSession(string userName)
    {
        return _tokenService.CreateSession(userName);
    }
    /// <summary>
    /// Returns a User as Data after validating a token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<HandlerResult<User>> ValidateSession(string token)
    {
        var tokenResult = _tokenService.ValidateSession(token);
        return tokenResult switch
        {
            HandlerResult<string>.Success success => await GetUser(success.Data),
            HandlerResult<string>.Failure failure => HandlerResult<User>.Error(failure.ErrorMessage),
            _ => throw new NotImplementedException()
        };
    }
    /// <summary>
    /// Ends a session based on username
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public HandlerResult<string> EndSession(string token)
    {
        return  _tokenService.EndSession(token);
    }
}
