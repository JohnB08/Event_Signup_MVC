using System;
using EventSignupApi.Models.HandlerResult;

namespace EventSignupApi.Services;

public class TokenService
{
    private readonly Dictionary<string, string> _activeSessions = [];
    /// <summary>
    /// Creates a new session in the active sessions dictionary based on the username. returns the generated token as Data.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public string CreateSession(string username)
    {
        string token = Guid.NewGuid().ToString();
        _activeSessions[token] = username;
        return token;
    }

    /// <summary>
    /// Validates a session based on an existing token.
    /// returns the username as Data
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public HandlerResult<string>ValidateSession(string token)
    {
        return  new HandlerResult<string>()
        {
            Success = _activeSessions.TryGetValue(token, out var userName),
            Data = userName ?? string.Empty,
            ErrorMessage = "Failed to load"
        };
    }

    /// <summary>
    /// Removes a token/username pair from the activeSessions dictionary. 
    /// returns action message. 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
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
