using EventSignupApi.Models.DTO;
using EventSignupApi.Models.HandlerResult;
using EventSignupApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventSignupApi.Controllers
{
    [Route("[controller]")] //localhost:5293/Signup
    [ApiController]
    public class SignupController(IWebHostEnvironment env, UserHandler userHandler) : ControllerBase
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly UserHandler _userHandler = userHandler;
        [HttpGet()]
        public IActionResult Get()
        {
            return PhysicalFile(Path.Combine(_env.WebRootPath,"signup.html"), "text/html");
        }
        [HttpPost()]
        public async Task<IActionResult> Post([FromForm] UserDto dto)
        {
            var result = await _userHandler.CreateNewUser(dto);
            switch (result)
            {
                case HandlerResult<string>.Success s:
                    Response.Cookies.Append("session_token", s.Data);
                    return Redirect("/");
                case HandlerResult<string>.Failure f:
                    return StatusCode(500, new {message = f.ErrorMessage});
                default:
                    return StatusCode(500, new {message = "something went wrong"});
            }
        }
    }
}
