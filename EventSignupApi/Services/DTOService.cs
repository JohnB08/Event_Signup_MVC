using System;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;

namespace EventSignupApi.Services;

public class DTOService
{
    public Event DtoHandler(EventDTO dto)
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

}
