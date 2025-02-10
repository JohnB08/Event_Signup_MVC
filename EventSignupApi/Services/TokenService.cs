using System;
using System.Collections.Concurrent;
using EventSignupApi.Models.HandlerResult;

namespace EventSignupApi.Services;

public class TokenService
{
    private readonly ConcurrentDictionary<string, string> _activeSessions = [];
    /// <summary>
    /// Creates a new session in the active sessions dictionary based on the username. returns the generated token as Data.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public string CreateSession(string username)
    {
        var token = Guid.NewGuid().ToString();
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
        return  _activeSessions.TryGetValue(token, out var userName) 
        ? HandlerResult<string>.Ok(userName) 
        : HandlerResult<string>.Error("Failed to find user");
    }

    /// <summary>
    /// Removes a token/username pair from the activeSessions dictionary. 
    /// returns action message. 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public HandlerResult<string> EndSession(string token)
    {
            return _activeSessions.Remove(token, out var _)
            ? HandlerResult<string>.Ok("Token removed successfully")
            : HandlerResult<string>.Error($"Failed to remove token.");
    }
}
