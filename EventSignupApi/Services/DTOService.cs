using System;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;

namespace EventSignupApi.Services;

public class DTOService
{
    public Event GetNewEvent(EventDTO dto, User user)
    {
        var newEvent = new Event(){
            EventName = dto.EventName,
            Public = dto.Public,
            UserId = user.UserId,
            Owner = user,
            MaxAttendees = dto.MaxAttendees,
        };
        if (DateTime.TryParse(dto.Date, out DateTime dtoDate))
        {
            newEvent.EventDate = dtoDate;
        }
        return newEvent;
    }
    public EventDTO MapEventToDto(Event returnEvent, bool canEdit = false)
    {
        var dto = new EventDTO()
        {
            EventName = returnEvent.EventName,
            Date = returnEvent.EventDate.ToString(),
            Public = returnEvent.Public,
            Genre = returnEvent.Genre.Genre,
            CanEdit = canEdit,
            MaxAttendees = returnEvent.MaxAttendees
        };
        return dto;
    }
    public void MapDtoToEvent(Event e, EventDTO dto, EventGenreLookupTable genre)
    {
        e.EventName = dto.EventName;
        e.EventDate = DateTime.Parse(dto.Date);
        e.Genre = genre;
        e.GenreId = genre.Id;
        e.MaxAttendees = dto.MaxAttendees;
    }

}
