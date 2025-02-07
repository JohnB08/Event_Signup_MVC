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
    public async Task<HandlerResult<EventDTO>> GetSingleEvent(int id, User user)
    {
        var e = await _context.Events.Include(e=> e.Genre).Include(e => e.Admins).Include(e => e.Owner).Where(e => e.Owner.UserId == user.UserId && e.EventId == id).FirstOrDefaultAsync();
        if (e != null && (e.Owner.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId))) return HandlerResult<EventDTO>.Ok(_dtoService.MapEventToDto(e, e.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId)));
        return HandlerResult<EventDTO>.Error("Failed fetching user");
    }
    public async Task<HandlerResult<string>> PostNewEvent(EventDTO dto, User user)
    {
        try
        {   
            
            var existingGenre = await _context.EventGenreLookup.Where(g => 
                                                            string.Equals(g.Genre, dto.Genre))
                                                            .FirstOrDefaultAsync();
            if (existingGenre == null)
            {
                var newGenre = new EventGenreLookupTable(){Genre = dto.Genre};
                _context.EventGenreLookup.Add(newGenre);
                _context.Events.Add(_dtoService.GetNewEvent(dto, user, newGenre));
                await _context.SaveChangesAsync();
                var e = await _context.Events.Where(e=> e.UserId == user.UserId).FirstOrDefaultAsync()!;
                user.EventId = e.EventId;
                user.OwnedEvent = e;
                await _context.SaveChangesAsync();
            }
            else 
            {
                _context.Events.Add(_dtoService.GetNewEvent(dto, user, existingGenre));
                await _context.SaveChangesAsync();
                var e = await _context.Events.Where(e=> e.UserId == user.UserId).FirstOrDefaultAsync()!;
                user.EventId = e.EventId;
                user.OwnedEvent = e;
                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();
            return HandlerResult<string>.Ok("Created new event!");
        }
        catch (Exception ex)
        {
            return HandlerResult<string>.Error($"Failed to create event: {ex.Message}");
        }
    }
    public async Task<HandlerResult<string>> EditEvent(EventDTO dto, int eventId)
    {
        try
        {
            var existingEvent = await _context.Events.FindAsync(eventId) ?? throw new NullReferenceException($"Missing event with event id = {eventId}");
            var dtoGenre = await _context.EventGenreLookup.Where(g => g.Genre == dto.Genre).FirstOrDefaultAsync();
            if (dtoGenre == null) 
            {
                dtoGenre = new (){Genre = dto.Genre};
                _context.Add(dtoGenre);
            }
            _dtoService.MapDtoToEvent(existingEvent, dto, dtoGenre);
            await _context.SaveChangesAsync();
            return HandlerResult<string>.Ok("Successfully edited event.");
        } catch (Exception ex)
        {
            return HandlerResult<string>.Error($"Failed to edit event: {ex.Message}");
        }
    }
    public async Task<HandlerResult<string>> DeleteEvent(int id,  User user)
    {
        try
        {
            var existingEvent = await _context.Events.Include(e=>e.Owner).Where(e => e.EventId == id && e.Owner.UserId == user.UserId).FirstOrDefaultAsync()!;
            _context.Events.Remove(existingEvent!);
            await _context.SaveChangesAsync();
            return HandlerResult<string>.Ok("Event successfully deleted.");
        }
        catch (Exception ex)
        {
            return HandlerResult<string>.Error($"Failed to delete event {ex.Message}");
        }

    }
}
