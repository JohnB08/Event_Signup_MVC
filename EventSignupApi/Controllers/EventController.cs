using EventSignupApi.Context;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;
using EventSignupApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventSignupApi.Controllers
{
    [Route("api/[controller]")]//localhost:3500/api/event
    [ApiController]
    public class EventController( ILogger<EventController> logger, EventRelationService eventRelationService) : ControllerBase
    {
        private readonly ILogger _logger = logger;
        private readonly EventRelationService _eventRealtionService = eventRelationService;
        public IActionResult Get()
        {
            var result = _eventRealtionService.GetEvents();
            if (result.Success) return Ok(result.Data);
            else return StatusCode(500, new {message = result.ErrorMessage});   
        }
        [HttpPost]
        public IActionResult Post([FromBody] EventDTO dto)
        {
            var insert = _eventRealtionService.InsertNewEvent(dto);
            if (insert.Success) return Ok(insert.Data);
            else return StatusCode(500, insert.ErrorMessage);
        }
        [HttpGet("edit")]
        public IActionResult Edit()
        {
            _logger.Log(LogLevel.Warning, "Someone edited something");
            return Ok(new {message = "Hello from Event controller Edit action"});
        }
    }
}
