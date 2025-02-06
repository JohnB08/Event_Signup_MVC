using EventSignupApi.Context;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;
using EventSignupApi.Models.HandlerResult;

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
    public HandlerResult<string> CreateNewUser(UserDTO dto)
    {
        if (_context.Users.Any(u => u.UserName == dto.UserName))
        return HandlerResult<string>.Error("Username allready taken");
        try
        {
            var newUser = _dtoService.GetNewUser(dto);
            _context.Users.Add(newUser);
            _context.SaveChanges();
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
    public HandlerResult<string> ValidateUserDto(UserDTO dto)
    {
        var hashedValues = _dtoService.HashDTOValues(dto);
        var existingUser = _context.Users.Where(u => u.Hash == hashedValues.Password).FirstOrDefault();
        if (existingUser == null) return HandlerResult<string>.Error("Missing user");
        return HandlerResult<string>.Ok("User validated");
    }

    /// <summary>
    /// Gets a user based on username. 
    /// Private method used after validating a token.
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    private HandlerResult<User> GetUser(string userName)
    {
        var user = _context.Users.Where(u => u.UserName == userName).FirstOrDefault();
        if (user == null) return HandlerResult<User>.Error("Missing user");
        return HandlerResult<User>.Ok(user);
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
    public HandlerResult<User> ValidateSession(string token)
    {
        var tokenResult = _tokenService.ValidateSession(token);
        return tokenResult switch
        {
            HandlerResult<string>.Success success => GetUser(success.Data),
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
