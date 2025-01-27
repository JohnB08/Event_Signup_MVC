using System;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;

namespace EventSignupApi.Services;

public class DTOService
{
    public Event NewEvent(EventDTO dto)
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
    public EventDTO MapFromQuery(Event e)
    {
        var dto = new EventDTO()
        {
            EventName = e.EventName,
            Public = e.Public,
            Date = e.EventDate.ToString(),
            Genre = e.Genre.Genre,
            UserId = e.UserId
        };
        return dto;
    }

}
