using System;
using System.Linq.Expressions;
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
            return HandlerResult<IEnumerable<EventDTO>>.Ok(_context.Events.Where(e=> e.Public == true).Include(e => e.Genre).Select(e => _dtoService.MapEventToDto(e, false)));
        }
        catch(Exception ex)
        {
            return HandlerResult<IEnumerable<EventDTO>>.Error(ex.Message);
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
            return HandlerResult<IEnumerable<EventDTO>>.Ok(_context.Events
                                .Include(e => e.Genre)
                                .Include(e => e.Admins)
                                .Where(e => e.Public || e.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId))
                                .Select(e => _dtoService.MapEventToDto(e, e.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId))));
        }
        catch (Exception ex)
        {
            return HandlerResult<IEnumerable<EventDTO>>.Error(ex.Message);
        }
    }
    public HandlerResult<EventDTO> GetSingleEvent(int id, User user)
    {
        var e = _context.Events.Include(e=> e.Genre).Include(e => e.Admins).Include(e => e.Owner).Where(e => e.Owner.UserId == user.UserId && e.EventId == id).FirstOrDefault();
        if (e != null && (e.Owner.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId))) return HandlerResult<EventDTO>.Ok(_dtoService.MapEventToDto(e, e.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId)));
        return HandlerResult<EventDTO>.Error("Failed fetching user");
    }
    public HandlerResult<string> PostNewEvent(EventDTO dto, User user)
    {
        try
        {   
            
            var existingGenre = _context.EventGenreLookup.Where(g => 
                                                            string.Equals(g.Genre, dto.Genre))
                                                            .FirstOrDefault();
            if (existingGenre == null)
            {
                var newGenre = new EventGenreLookupTable(){Genre = dto.Genre};
                _context.EventGenreLookup.Add(newGenre);
                _context.Events.Add(_dtoService.GetNewEvent(dto, user, newGenre));
                _context.SaveChanges();
                var e = _context.Events.Where(e=> e.UserId == user.UserId).FirstOrDefault()!;
                user.EventId = e.EventId;
                user.OwnedEvent = e;
                _context.SaveChanges();
            }
            else 
            {
                _context.Events.Add(_dtoService.GetNewEvent(dto, user, existingGenre));
                _context.SaveChanges();
                var e = _context.Events.Where(e=> e.UserId == user.UserId).FirstOrDefault()!;
                user.EventId = e.EventId;
                user.OwnedEvent = e;
                _context.SaveChanges();
            }
            _context.SaveChanges();
            return HandlerResult<string>.Ok("Created new event!");
        }
        catch (Exception ex)
        {
            return HandlerResult<string>.Error($"Failed to create event: {ex.Message}");
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
            return HandlerResult<string>.Ok("Successfully edited event.");
        } catch (Exception ex)
        {
            return HandlerResult<string>.Error($"Failed to edit event: {ex.Message}");
        }
    }
    public HandlerResult<string> DeleteEvent(int id,  User user)
    {
        try
        {
            _context.Events.Remove(_context.Events.Include(e=>e.Owner).Where(e => e.EventId == id && e.Owner.UserId == user.UserId).FirstOrDefault()!);
            _context.SaveChanges();
            return HandlerResult<string>.Ok("Event successfully deleted.");
        }
        catch (Exception ex)
        {
            return HandlerResult<string>.Error($"Failed to delete event {ex.Message}");
        }

    }
}
