using System;
using EventSignupApi.Context;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;
using EventSignupApi.Models.HandlerResult;
using Microsoft.EntityFrameworkCore;


namespace EventSignupApi.Services;

public class EventDataHandler(DatabaseContext context, EventDTOService dtoService, ILogger<EventDataHandler> logger)
{
    private readonly DatabaseContext _context = context;
    private readonly EventDTOService _dtoService = dtoService;
    private readonly ILogger<EventDataHandler> _logger = logger;

    /* Overload of GetEvents that gets all publicly viewable events, and sets editable to false by default */
    public HandlerResult<IEnumerable<EventDTO>> GetEvents()
    {
        try
        {
            return new HandlerResult<IEnumerable<EventDTO>>()
            {
                Success = true,
                Data = _context.Events.Where(e=> e.Public == true).Include(e => e.Genre).Select(e => _dtoService.MapEventToDto(e, false))
            };
        }
        catch(Exception ex)
        {
            return new HandlerResult<IEnumerable<EventDTO>>()
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Get Events tied to a spesific user.
    /// On success returns in Data a map of dtos with CanEdit set to true if user is owner or admin. 
    /// Returns errormessage on failure. 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public HandlerResult<IEnumerable<EventDTO>> GetEvents(User user)
    {
        try
        {
            return new HandlerResult<IEnumerable<EventDTO>>()
            {
                Success = true,
                Data = _context.Events
                                .Include(e => e.Genre)
                                .Include(e => e.Admins)
                                .Where(e => e.Public || e.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId))
                                .Select(e => _dtoService.MapEventToDto(e, e.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId)))
            };
        }
        catch (Exception ex)
        {
            return new HandlerResult<IEnumerable<EventDTO>>()
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
    public HandlerResult<EventDTO> GetSingleEvent(int id, User user)
    {
        var e = _context.Events.Include(e=> e.Genre).Include(e => e.Admins).Include(e => e.Owner).Where(e => e.Owner.UserId == user.UserId && e.EventId == id).FirstOrDefault();
        if (e != null && (e.Owner.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId))) return new HandlerResult<EventDTO>()
        {
            Success = true,
            Data = _dtoService.MapEventToDto(e, e.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId))
        };
        return new HandlerResult<EventDTO>()
        {
            Success = false,
            ErrorMessage = "Event not found"
        };
    }
    public HandlerResult<string> PostNewEvent(EventDTO dto, User user)
    {
        try
        {   
            var newEvent = _dtoService.GetNewEvent(dto, user);
            var existingGenre = _context.EventGenreLookup.Where(g => 
                                                                string.Equals(g.Genre, dto.Genre))
                                                            .FirstOrDefault();
            if (existingGenre == null)
            {
                var newGenre = new EventGenreLookupTable(){Genre = dto.Genre};
                _context.EventGenreLookup.Add(newGenre);
                newEvent.GenreId = newGenre.Id;
                newEvent.Genre = newGenre;
            }
            else 
            {
                newEvent.Genre = existingGenre;
                newEvent.GenreId = existingGenre.Id;
            }
            var existingUser = _context.Users.Where(u => u.UserId == newEvent.UserId).FirstOrDefault();
            existingUser.OwnedEvent = newEvent;
            existingUser.EventId = newEvent.EventId;
            newEvent.Owner = existingUser;
            newEvent.UserId = existingUser.UserId;
            _context.Events.Add(newEvent);
            _context.SaveChanges();
            return new HandlerResult<string>()
            {
                Success = true,
                Data = $"Successfully created new event with name {newEvent.EventName}"
            };
        }
        catch (Exception ex)
        {
            return new HandlerResult<string>()
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
    public HandlerResult<string> EditEvent(EventDTO dto, int eventId)
    {
        try
        {
            var existingEvent = _context.Events.Find(eventId) ?? throw new NullReferenceException($"Missing event with event id = {eventId}");
            var dtoGenre = _context.EventGenreLookup.Where(g => g.Genre == dto.Genre).FirstOrDefault();
            if (dtoGenre == null) 
            {
                dtoGenre = new (){Genre = dto.Genre};
                _context.Add(dtoGenre);
            }
            _dtoService.MapDtoToEvent(existingEvent, dto, dtoGenre);
            _context.SaveChanges();
            return new HandlerResult<string>()
            {
                Success = true,
                Data = "Edit Successfull"
            };
        } catch (Exception ex)
        {
            return new HandlerResult<string>()
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
