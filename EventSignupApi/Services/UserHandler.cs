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
        return new HandlerResult<string>()
        {
            Success = false,
            ErrorMessage = "Username allready taken"
        };
        try
        {
            var newUser = _dtoService.GetNewUser(dto);
            _context.Users.Add(newUser);
            _context.SaveChanges();
            var token  = _tokenService.CreateSession(newUser.UserName);
            return new HandlerResult<string>()
            {
                Success = true,
                Data = token
            };
        }
        catch (Exception ex)
        {
            return new HandlerResult<string>
            {
                Success = false,
                ErrorMessage = ex.Message
            };
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
        if (existingUser == null) return new HandlerResult<string>()
        {
            Success = false,
            ErrorMessage = "Could not find a valid user"
        };
        return new HandlerResult<string>()
        {
            Success = true,
            Data = $"User with username: {existingUser.UserName} found."
        };
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
        if (user == null) return new HandlerResult<User>()
        {
            Success = false,
            ErrorMessage = "Missing User"
        };
        return new HandlerResult<User>()
        {
            Success = true,
            Data = user
        };
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
        if (!tokenResult.Success) return new HandlerResult<User>()
        {
            Success = false,
            ErrorMessage = tokenResult.ErrorMessage
        };
        return GetUser(tokenResult.Data);
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
