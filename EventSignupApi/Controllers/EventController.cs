using System.Threading.Tasks;
using EventSignupApi.Models;
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

        public async Task<IActionResult> Get()
        {
            if (Request.Cookies.TryGetValue("session_token", out var token))
            {
                var userResult = await _userHandler.ValidateSession(token);
                if (userResult is HandlerResult<User>.Success success)
                {
                    return _eventDataHandler.GetEvents(success.Data) switch
                    {
                        HandlerResult<IEnumerable<EventDTO>>.Success s => Ok(s.Data),
                        HandlerResult<IEnumerable<EventDTO>>.Failure f => StatusCode(500, new {message = f.ErrorMessage}),
                        _ => StatusCode(500, new {message = "something went wrong"})
                    };
                }
            }
            return _eventDataHandler.GetEvents() switch
            {
                    HandlerResult<IEnumerable<EventDTO>>.Success s => Ok(s.Data),
                    HandlerResult<IEnumerable<EventDTO>>.Failure f => StatusCode(500, new {message = f.ErrorMessage}),
                    _ => StatusCode(500, new {message = "Something went wrong"})
            };
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!Request.Cookies.TryGetValue("session_token", out var token))
            {
                return Unauthorized(new {message = "No access to single event."});
            }
            var userResult = await _userHandler.ValidateSession(token);
            if (userResult is HandlerResult<User>.Success s)
            {
                return await _eventDataHandler.GetSingleEvent(id, s.Data) switch
                {
                    HandlerResult<EventDTO>.Success su => Ok(su.Data),
                    HandlerResult<EventDTO>.Failure f => StatusCode(500, new {message = f.ErrorMessage}),
                    _ => StatusCode(500, new {message = "something went wrong"})
                };
            }
            else return Unauthorized();

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!Request.Cookies.TryGetValue("session_token", out var token))
            {
                return Unauthorized(new {message = "No access to single event."});
            }
            var userResult = await _userHandler.ValidateSession(token);
            if (userResult is HandlerResult<User>.Success s)
            {
                return await _eventDataHandler.DeleteEvent(id, s.Data) switch
                {
                    HandlerResult<string>.Success su => Ok(su.Data),
                    HandlerResult<string>.Failure f => StatusCode(500, new {message = f.ErrorMessage}),
                    _ => StatusCode(500, new {message = "Something went wrong"})
                };
            }
            return StatusCode(500, new {message = "Something went wrong"});
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EventDTO dto)
        {
            if (Request.Cookies.TryGetValue("session_token", out var token))
            {
                var userResult = await _userHandler.ValidateSession(token);
                if (userResult is HandlerResult<User>.Success s)
                {
                    return await _eventDataHandler.PostNewEvent(dto, s.Data) switch
                    {
                        HandlerResult<string>.Success su => Ok(su.Data),
                        HandlerResult<string>.Failure f => StatusCode(500, new {message = f.ErrorMessage}),
                        _ => StatusCode(500, new {message = "Something went wrong"})
                    };
                }
            }
            return Unauthorized(new {message = "Missing session token"});
        }
        [HttpGet("edit/{id}")]
        public IActionResult GetEdit()
        {
            return PhysicalFile(Path.Combine(_env.WebRootPath, "edit.html"), "text/html");
        }
        [HttpPatch("edit/{id}")]
        public async Task<IActionResult> PatchEvent([FromRoute]int id, [FromBody] EventDTO dto)
        {
            if (!Request.Cookies.TryGetValue("session_token", out var token))
            {
                return Unauthorized(new {message = "Unauthorized access"});
            }
            return  await _eventDataHandler.EditEvent(dto, id) switch
            {
                HandlerResult<string>.Success s => Ok(s.Data),
                HandlerResult<string>.Failure f => StatusCode(500, new {message = f.ErrorMessage}),
                _ => StatusCode(500, new {message = "Something went wrong"})
            };
        }
        [HttpGet("create")]
        public IActionResult Create()
        {
            return PhysicalFile(Path.Combine(_env.WebRootPath, "create.html"), "text/html");
        }
    }
}
