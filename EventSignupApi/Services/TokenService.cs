using System;
using EventSignupApi.Models.HandlerResult;

namespace EventSignupApi.Services;

public class TokenService
{
    private readonly Dictionary<string, string> _activeSessions = [];
    public string CreateSession(string username)
    {
        string token = Guid.NewGuid().ToString();
        _activeSessions[token] = username;
        return token;
    }
    public HandlerResult<string>ValidateSession(string token)
    {
        return _activeSessions.TryGetValue(token, out string userName) ? new HandlerResult<string>(){ Success = true, Data = userName} : new HandlerResult<string>(){Success = false, ErrorMessage = "Invalid Token"};
    }
    public HandlerResult<string> EndSession(string token)
    {
        try
        {
            _activeSessions.Remove(token);
            return new HandlerResult<string>()
            {
                Success = true,
                Data = "Token removed successfully"
            };
        }
        catch (Exception ex)
        {
            return new HandlerResult<string>()
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
