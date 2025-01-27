using System;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;

namespace EventSignupApi.Services;

public class DTOService
{
    public Event GetNewEvent(EventDTO dto)
    {
        var newEvent = new Event(){
            EventName = dto.EventName,
            Public = dto.Public,
            UserId = dto.UserId
        };
        if (DateTime.TryParse(dto.Date, out DateTime dtoDate))
        {
            newEvent.EventDate = dtoDate;
        }
        return newEvent;
    }
    public EventDTO MapEventToDto(Event returnEvent)
    {
        var dto = new EventDTO()
        {
            EventName = returnEvent.EventName,
            Date = returnEvent.EventDate.ToString(),
            Public = returnEvent.Public,
            Genre = returnEvent.Genre.Genre
        };
        return dto;
    }

}
