using EventSignupApi.Context;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;
using EventSignupApi.Models.HandlerResult;
using EventSignupApi.Services.LevenShteinService;
using Microsoft.EntityFrameworkCore;


namespace EventSignupApi.Services;

public class EventDataHandler(DatabaseContext context, EventDtoService dtoService, LS ls)
{

    /* Overload of GetEvents that gets all publicly viewable events, and sets editable to false by default */
    public HandlerResult<IEnumerable<EventDTO>> GetEvents()
    {
        try
        {
            return HandlerResult<IEnumerable<EventDTO>>.Ok(context.Events.Where(e=> e.Public == true).Include(e => e.Genre).Select(e => EventDtoService.MapEventToDto(e, false)));
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
            return HandlerResult<IEnumerable<EventDTO>>.Ok(context.Events
                                .Include(e => e.Genre)
                                .Include(e => e.Admins)
                                .Where(e => e.Admins != null && (e.Public || e.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId)))
                                .Select(e => EventDtoService.MapEventToDto(e, e.Admins != null && (e.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId)))));
        }
        catch (Exception ex)
        {
            return HandlerResult<IEnumerable<EventDTO>>.Error(ex.Message);
        }
    }
    public async Task<HandlerResult<EventDTO>> GetSingleEvent(int id, User user)
    {
        var e = await context.Events.Include(e=> e.Genre).Include(e => e.Admins).Include(e => e.Owner).Where(e => e.Owner.UserId == user.UserId && e.EventId == id).FirstOrDefaultAsync();
        if (e is { Admins: not null } && (e.Owner.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId))) return HandlerResult<EventDTO>.Ok(EventDtoService.MapEventToDto(e, e.UserId == user.UserId || e.Admins.Any(a => a.UserId == user.UserId)));
        return HandlerResult<EventDTO>.Error("Failed fetching user");
    }
    public async Task<HandlerResult<string>> PostNewEvent(EventDTO dto, User user)
    {
        try
        {   
            
            var genreList = await context.EventGenreLookup.ToListAsync();
            var existingGenre = genreList.AsParallel()
                                                                .Select(g => new {
                                                                    genreObj = g,
                                                                    distance = LS.DistanceRec(dto.Genre.ToLower().AsSpan(), g.Genre.ToLower().AsSpan())
                                                                })
                                                                .Where(x => x.distance < (x.genreObj.Genre.Length/2)+1)
                                                                .OrderBy(x => x.distance)
                                                                .Select(x => x.genreObj)
                                                                .FirstOrDefault();
            if (existingGenre == null)
            {
                var newGenre = new EventGenreLookupTable(){Genre = dto.Genre};
                context.EventGenreLookup.Add(newGenre);
                context.Events.Add(EventDtoService.GetNewEvent(dto, user, newGenre));
                await context.SaveChangesAsync();
                var e = await context.Events.Where(e=> e.UserId == user.UserId).FirstOrDefaultAsync();
                user.EventId = e!.EventId;
                user.OwnedEvent = e;
            }
            else 
            {
                context.Events.Add(EventDtoService.GetNewEvent(dto, user, existingGenre));
                await context.SaveChangesAsync();
                var e = await context.Events.Where(e=> e.UserId == user.UserId).FirstOrDefaultAsync();
                user.EventId = e!.EventId;
                user.OwnedEvent = e;
            }
            await context.SaveChangesAsync();
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
            var existingEvent = await context.Events.Where(e => e.EventId == eventId).FirstOrDefaultAsync();
            var existingGenres = await context.EventGenreLookup.ToListAsync();
            var dtoGenre = existingGenres.AsParallel()
                                                                .Select(g => new {
                                                                    genreObj = g,
                                                                    distance = LS.DistanceRec(dto.Genre.ToLower().AsSpan(), g.Genre.ToLower().AsSpan())
                                                                })
                                                                .Where(x => x.distance < (x.genreObj.Genre.Length/2)+1)
                                                                .OrderBy(x => x.distance)
                                                                .Select(x => x.genreObj)
                                                                .FirstOrDefault();
                                                                                    
            if (dtoGenre == null) 
            {
                dtoGenre = new (){Genre = dto.Genre};
                context.Add(dtoGenre);
            }
            EventDtoService.MapDtoToEvent(existingEvent!, dto, dtoGenre);
            await context.SaveChangesAsync();
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
            var existingEvent = await context.Events.Include(e=>e.Owner).Where(e => e.EventId == id && e.Owner.UserId == user.UserId).FirstOrDefaultAsync();
            context.Events.Remove(existingEvent!);
            await context.SaveChangesAsync();
            return HandlerResult<string>.Ok("Event successfully deleted.");
        }
        catch (Exception ex)
        {
            return HandlerResult<string>.Error($"Failed to delete event {ex.Message}");
        }

    }
}
