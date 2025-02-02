using EventSignupApi.Models.DTO;
using EventSignupApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventSignupApi.Controllers
{
    [Route("[controller]")] //localhost:5293/login
    [ApiController]
    public class LoginController(IWebHostEnvironment env, UserHandler userHandler) : ControllerBase
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly UserHandler _userHandler = userHandler;
        [HttpGet()]
        public IActionResult Get()
        {
            return PhysicalFile(Path.Combine(_env.WebRootPath, "login.html"), "text/html");
        }
        [HttpPost()]
        public IActionResult Post([FromForm] UserDTO dto)
        {
            var result = _userHandler.ValidateUserDto(dto);
            if (!result.Success) return Unauthorized(result.ErrorMessage);

            string token = _userHandler.CreateSession(dto.UserName);

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
        public IActionResult IsAuthenticated()
        {
            if (Request.Cookies.TryGetValue("session_token", out var sessionToken))
            {
                var result = _userHandler.ValidateSession(sessionToken);
                if (result.Success) return Ok(new {username = result.Data.UserName});
                return Unauthorized(new {message = result.ErrorMessage});
            }
            return Unauthorized(new {message = "no token"});
        }
        [HttpPost("signout")]
        public IActionResult Signout()
        {
            if (Request.Cookies.TryGetValue("session_token", out var sessionToken))
            {
                _userHandler.EndSession(sessionToken);
                Response.Cookies.Delete("session_token");
            }
            return Redirect("/");
        }
    }
}
