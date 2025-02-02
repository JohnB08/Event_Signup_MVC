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
            _context.Users.Add(_dtoService.GetNewUser(dto));
            _context.SaveChanges();
            return new HandlerResult<string>()
            {
                Success = true,
                Data = $"User with username {dto.UserName} created!"
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
    public string CreateSession(string userName)
    {
        return _tokenService.CreateSession(userName);
    }
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
    public HandlerResult<string> EndSession(string token)
    {
        return  _tokenService.EndSession(token);
    }
}
