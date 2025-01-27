using EventSignupApi.Context;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;
using EventSignupApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace EventSignupApi.Controllers
{
    [Route("api/[controller]")]//localhost:3500/api/event
    [ApiController]
    public class EventController(ILogger<EventController> logger, EventDataHandler eventDataHandler) : ControllerBase
    {

        private readonly ILogger _logger = logger;
        private readonly EventDataHandler _eventDataHandler = eventDataHandler;

        public IActionResult Get()
        {
            var result = _eventDataHandler.GetEvents();
            if (result.Success) return Ok(result.Data);
            return StatusCode(500, new {message = result.ErrorMessage});
        }
        [HttpPost]
        public IActionResult Post([FromBody] EventDTO dto)
        {
            var result = _eventDataHandler.PostNewEvent(dto);
            if (result.Success) return Ok(new {message = result.Data});
            return StatusCode(500, new {message = result.ErrorMessage});
        }
        [HttpGet("edit")]
        public IActionResult Edit()
        {
            _logger.Log(LogLevel.Warning, "Someone edited something");
            return Ok(new {message = "Hello from Event controller Edit action"});
        }
    }
}
