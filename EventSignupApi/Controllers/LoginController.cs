using EventSignupApi.Models;
using EventSignupApi.Models.DTO;
using EventSignupApi.Models.HandlerResult;
using EventSignupApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventSignupApi.Controllers
{
    [Route("[controller]")] //localhost:5293/login
    [ApiController]
    public class LoginController(IWebHostEnvironment env, UserHandler userHandler) : ControllerBase
    {
        [HttpGet()]
        public IActionResult Get()
        {
            return PhysicalFile(Path.Combine(env.WebRootPath, "login.html"), "text/html");
        }
        [HttpPost()]
        public async Task<IActionResult> Post([FromForm] UserDTO dto)
        {
            var result = await userHandler.ValidateUserDto(dto);
            if (result is HandlerResult<string>.Failure f) return Unauthorized(new {message = f.ErrorMessage});

            var token = userHandler.CreateSession(dto.UserName);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddHours(1)
            };
            Response.Cookies.Append("session_token", token, cookieOptions);

            return Redirect("/");
        }
        [HttpGet("IsAuthenticated")]
        public async Task<IActionResult> IsAuthenticated()
        {
            if (Request.Cookies.TryGetValue("session_token", out var sessionToken))
            {
                return await userHandler.ValidateSession(sessionToken) switch
                {
                    HandlerResult<User>.Success s => Ok(new {userName = s.Data.UserName}),
                    HandlerResult<User>.Failure f => Unauthorized(new {message = f.ErrorMessage}),
                    _ => StatusCode(500, new {message = "Something went wrong"})
                };
            }
            return Unauthorized(new {message = "no token"});
        }
        [HttpPost("SignOut")]
        public IActionResult LogOut()
        {
            if (!Request.Cookies.TryGetValue("session_token", out var sessionToken)) return Redirect("/");
            userHandler.EndSession(sessionToken);
            Response.Cookies.Delete("session_token");
            return Redirect("/");
        }
    }
}
