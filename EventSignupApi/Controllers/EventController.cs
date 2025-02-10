using EventSignupApi.Models;
using EventSignupApi.Models.DTO;
using EventSignupApi.Models.HandlerResult;
using EventSignupApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventSignupApi.Controllers
{
    [Route("[controller]")]//localhost:3500/event
    [ApiController]
    public class EventController(EventDataHandler eventDataHandler, IWebHostEnvironment env, UserHandler userHandler) : ControllerBase
    {
        public async Task<IActionResult> Get()
        {
            if (!Request.Cookies.TryGetValue("session_token", out var token))
                return eventDataHandler.GetEvents() switch
                {
                    HandlerResult<IEnumerable<EventDTO>>.Success s => Ok(s.Data),
                    HandlerResult<IEnumerable<EventDTO>>.Failure f => StatusCode(500, new { message = f.ErrorMessage }),
                    _ => StatusCode(500, new { message = "Something went wrong" })
                };
            var userResult = await userHandler.ValidateSession(token);
            if (userResult is HandlerResult<User>.Success success)
            {
                return eventDataHandler.GetEvents(success.Data) switch
                {
                    HandlerResult<IEnumerable<EventDTO>>.Success s => Ok(s.Data),
                    HandlerResult<IEnumerable<EventDTO>>.Failure f => StatusCode(500, new {message = f.ErrorMessage}),
                    _ => StatusCode(500, new {message = "something went wrong"})
                };
            }
            return eventDataHandler.GetEvents() switch
            {
                    HandlerResult<IEnumerable<EventDTO>>.Success s => Ok(s.Data),
                    HandlerResult<IEnumerable<EventDTO>>.Failure f => StatusCode(500, new {message = f.ErrorMessage}),
                    _ => StatusCode(500, new {message = "Something went wrong"})
            };
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!Request.Cookies.TryGetValue("session_token", out var token))
            {
                return Unauthorized(new {message = "No access to single event."});
            }
            var userResult = await userHandler.ValidateSession(token);
            if (userResult is HandlerResult<User>.Success s)
            {
                return await eventDataHandler.GetSingleEvent(id, s.Data) switch
                {
                    HandlerResult<EventDTO>.Success su => Ok(su.Data),
                    HandlerResult<EventDTO>.Failure f => StatusCode(500, new {message = f.ErrorMessage}),
                    _ => StatusCode(500, new {message = "something went wrong"})
                };
            }
            else return Unauthorized();

        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!Request.Cookies.TryGetValue("session_token", out var token))
            {
                return Unauthorized(new {message = "No access to single event."});
            }
            var userResult = await userHandler.ValidateSession(token);
            if (userResult is HandlerResult<User>.Success s)
            {
                return await eventDataHandler.DeleteEvent(id, s.Data) switch
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
            if (!Request.Cookies.TryGetValue("session_token", out var token))
                return Unauthorized(new { message = "Missing session token" });
            var userResult = await userHandler.ValidateSession(token);
            if (userResult is HandlerResult<User>.Success s)
            {
                return await eventDataHandler.PostNewEvent(dto, s.Data) switch
                {
                    HandlerResult<string>.Success su => Ok(su.Data),
                    HandlerResult<string>.Failure f => StatusCode(500, new {message = f.ErrorMessage}),
                    _ => StatusCode(500, new {message = "Something went wrong"})
                };
            }
            return Unauthorized(new {message = "Missing session token"});
        }
        [HttpGet("edit/{id}")]
        public IActionResult GetEdit(string id)
        {
            return PhysicalFile(Path.Combine(env.WebRootPath, "edit.html"), "text/html");
        }
        [HttpPatch("edit/{id:int}")]
        public async Task<IActionResult> PatchEvent([FromRoute]int id, [FromBody] EventDTO dto)
        {
            if (!Request.Cookies.TryGetValue("session_token", out var _))
            {
                return Unauthorized(new {message = "Unauthorized access"});
            }
            return  await eventDataHandler.EditEvent(dto, id) switch
            {
                HandlerResult<string>.Success s => Ok(s.Data),
                HandlerResult<string>.Failure f => StatusCode(500, new {message = f.ErrorMessage}),
                _ => StatusCode(500, new {message = "Something went wrong"})
            };
        }
        [HttpGet("create")]
        public IActionResult Create()
        {
            return PhysicalFile(Path.Combine(env.WebRootPath, "create.html"), "text/html");
        }
    }
}
