using System;
using EventSignupApi.Context;
using EventSignupApi.Models;
using EventSignupApi.Models.DTO;
using EventSignupApi.Models.HandlerResult;
using Microsoft.EntityFrameworkCore;


namespace EventSignupApi.Services;

public class EventDataHandler(DatabaseContext context, DTOService dtoService)
{
    private readonly DatabaseContext _context = context;
    private readonly DTOService _dtoService = dtoService;

    public HandlerResult<IEnumerable<EventDTO>> GetEvents()
    {
        try
        {
            return new HandlerResult<IEnumerable<EventDTO>>()
            {
                Success = true,
                Data = _context.Events.Include(e => e.Genre).Select(e => _dtoService.MapEventToDto(e))
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
    public HandlerResult<string> PostNewEvent(EventDTO dto)
    {
        try
        {
            var newEvent = _dtoService.GetNewEvent(dto);
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
}
