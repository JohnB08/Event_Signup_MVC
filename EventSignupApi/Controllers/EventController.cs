using EventSignupApi.Context;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;
using EventSignupApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventSignupApi.Controllers
{
    [Route("api/[controller]")]//localhost:3500/api/event
    [ApiController]
    public class EventController(DatabaseContext context, ILogger<EventController> logger, DTOService dtoService) : ControllerBase
    {
        private readonly DatabaseContext _context = context;
        private readonly ILogger _logger = logger;
        private readonly DTOService _dtoService = dtoService;
        public IActionResult Get()
        {
            try 
            {
                return Ok(_context.Events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {message = ex.Message});
            }
        }
        [HttpPost]
        public IActionResult Post([FromBody] EventDTO dto)
        {
            try 
            {
                var newEvent = _dtoService.DtoHandler(dto);
            var existingGenre = _context.EventGenreLookup.Where(g => 
                                                                string.Equals(g.Genre, dto.Genre))
                                                            .FirstOrDefault();
            if (existingGenre == null)
            {
                var newGenre = new EventGenreLookupTable(){Genre = dto.Genre};
                _context.EventGenreLookup.Add(newGenre);
                newEvent.Genre = newGenre;
            }
            else 
            {
                newEvent.Genre = existingGenre;
            }
            var existingUser = _context.Users.Where(u => u.UserId == newEvent.UserId).FirstOrDefault();
            existingUser.OwnedEvent = newEvent;
            existingUser.EventId = newEvent.EventId;
            newEvent.Owner = existingUser;
            _context.Events.Add(newEvent);
            _context.SaveChanges();
            return Ok(new {message = $"Event with name {newEvent.EventName} created"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {message = $"Soemthing went wrong: {ex.Message}"});
            }
        }
        [HttpGet("edit")]
        public IActionResult Edit()
        {
            _logger.Log(LogLevel.Warning, "Someone edited something");
            return Ok(new {message = "Hello from Event controller Edit action"});
        }
    }
}
