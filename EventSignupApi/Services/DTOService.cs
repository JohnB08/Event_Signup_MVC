using System.Globalization;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;

namespace EventSignupApi.Services;

public class EventDtoService
{
    /// <summary>
    /// Creates a new event based on DTO and a User.
    /// Will fail if user allready has an event tied to them as owner. 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="user"></param>
    /// <param name="genre"></param>
    /// <returns></returns>
    public static Event GetNewEvent(EventDTO dto, User user, EventGenreLookupTable genre)
    {
        var newEvent = new Event()
        {
            EventName = dto.EventName,
            Public = dto.Public,
            UserId = user.UserId,
            Owner = user,
            MaxAttendees = dto.MaxAttendees,
            Lat = dto.LatLong[0],
            Long = dto.LatLong[1],
            GenreId = genre.Id,
            Genre = genre,
            EventDate = DateTime.TryParse(dto.Date, out var dtoDate) ? dtoDate : DateTime.Now
        };
        return newEvent;
    }
    /// <summary>
    /// Maps an Event object to a DTO
    /// canEdit defaults to false
    /// </summary>
    /// <param name="returnEvent"></param>
    /// <param name="canEdit"></param>
    /// <returns></returns>
    public static EventDTO MapEventToDto(Event returnEvent, bool canEdit = false)
    {
        var dto = new EventDTO()
        {
            Id = returnEvent.EventId,
            EventName = returnEvent.EventName,
            Date = returnEvent.EventDate.ToString(CultureInfo.InvariantCulture),
            Public = returnEvent.Public,
            Genre = returnEvent.Genre.Genre,
            CanEdit = canEdit,
            MaxAttendees = returnEvent.MaxAttendees,
            LatLong = [returnEvent.Lat, returnEvent.Long]
        };
        return dto;
    }
    /// <summary>
    /// Maps a DTO to an existing event. needs the Genre tied to event. 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="dto"></param>
    /// <param name="genre"></param>
    public static void MapDtoToEvent(Event e, EventDTO dto, EventGenreLookupTable genre)
    {
        e.EventName = dto.EventName;
        e.EventDate = DateTime.Parse(dto.Date);
        e.Genre = genre;
        e.GenreId = genre.Id;
        e.Public = dto.Public;
        e.MaxAttendees = dto.MaxAttendees;
        e.Lat = dto.LatLong[0];
        e.Long = dto.LatLong[1];
    }

}
