using System.Runtime.InteropServices;
using EventSignupApi.Models.DTO;
using EventSignupApi.Models.HandlerResult;
using EventSignupApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventSignupApi.Controllers
{
    [Route("[controller]")]//localhost:3500/event
    [ApiController]
    public class EventController(ILogger<EventController> logger, EventDataHandler eventDataHandler, IWebHostEnvironment env, UserHandler userHandler) : ControllerBase
    {

        private readonly ILogger _logger = logger;

        private readonly IWebHostEnvironment _env = env;
        private readonly EventDataHandler _eventDataHandler = eventDataHandler;
        private readonly UserHandler _userHandler = userHandler;

        public IActionResult Get()
        {
            if (Request.Cookies.TryGetValue("session_token", out var token))
            {
                var userResult = _userHandler.ValidateSession(token);
                if (userResult.Success) 
                {
                    var privateResults = _eventDataHandler.GetEvents(userResult.Data);
                    return privateResults.Success ? Ok(privateResults.Data) : StatusCode(500, new{ message = privateResults.ErrorMessage});
                }
            }
            var results = _eventDataHandler.GetEvents();
            return results.Success ? Ok(results.Data) : StatusCode(500, new {message = results.ErrorMessage});
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!Request.Cookies.TryGetValue("session_token", out var token))
            {
                return Unauthorized(new {message = "No access to single event."});
            }
            var userResult = _userHandler.ValidateSession(token);
            if (!userResult.Success) return Unauthorized(new {message = "No access to single event."});
            var eventResult = _eventDataHandler.GetSingleEvent(id, userResult.Data);
            if (!eventResult.Success) return StatusCode(500, new {message = eventResult.ErrorMessage});
            return Ok(eventResult.Data);
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!Request.Cookies.TryGetValue("session_token", out var token))
            {
                return Unauthorized(new {message = "No access to single event."});
            }
            var userResult = _userHandler.ValidateSession(token);
            if (!userResult.Success) return Unauthorized(new {message = "No access to single event"});
            var deleteResult = _eventDataHandler.DeleteEvent(id, userResult.Data);
            return deleteResult.Success ? Ok(deleteResult.Data) : StatusCode(500, new {message = deleteResult.ErrorMessage});
        }
        [HttpPost]
        public IActionResult Post([FromBody] EventDTO dto)
        {
            if (Request.Cookies.TryGetValue("session_token", out var token))
            {
                var userResult = _userHandler.ValidateSession(token);
                if (!userResult.Success) return Unauthorized(new {message = userResult.ErrorMessage});
                var result = _eventDataHandler.PostNewEvent(dto, userResult.Data);
                if (result.Success) return Ok(new {message = result.Data});
                return StatusCode(500, new {message = result.ErrorMessage});
            }
            return Unauthorized(new {message = "Missing session token"});
        }
        [HttpGet("edit/{id}")]
        public IActionResult GetEdit()
        {
            return PhysicalFile(Path.Combine(_env.WebRootPath, "edit.html"), "text/html");
        }
        [HttpPatch("edit/{id}")]
        public IActionResult PatchEvent([FromRoute]int id, [FromBody] EventDTO dto)
        {
            if (!Request.Cookies.TryGetValue("session_token", out var token))
            {
                return Unauthorized(new {message = "Unauthorized access"});
            }
            var result = _eventDataHandler.EditEvent(dto, id);
            return result.Success ? Ok(result.Data) : StatusCode(500, result.ErrorMessage);
        }
        [HttpGet("create")]
        public IActionResult Create()
        {
            return PhysicalFile(Path.Combine(_env.WebRootPath, "create.html"), "text/html");
        }
    }
}
