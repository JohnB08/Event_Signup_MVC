using EventSignupApi.Models.DTO;
using EventSignupApi.Services;
using Microsoft.AspNetCore.Http;
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
        public IActionResult Post([FromForm] UserDTO dto)
        {
            var result = _userHandler.CreateNewUser(dto);
            if (result.Success) 
            {   
                Response.Cookies.Append("session_token", result.Data);
                return Redirect("/");
            }
            return StatusCode(500, result.ErrorMessage);

        }
    }
}
