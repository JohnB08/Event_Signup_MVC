using System;
using EventSignupApi.Context;
using EventSignupApi.Models.DTO;
using EventSignupApi.Models;
using Microsoft.AspNetCore.Mvc;
using EventSignupApi.Models.ServiceResult;
using Microsoft.EntityFrameworkCore;

namespace EventSignupApi.Services;

public class EventRelationService(DatabaseContext context, DTOService dtoService)
{
    private readonly DatabaseContext _context = context;
    private readonly DTOService _dtoService = dtoService;

    public ServiceResult<string> InsertNewEvent(EventDTO dto)
    {
        try 
            {
                var newEvent = _dtoService.NewEvent(dto);
                var existingGenre = _context.EventGenreLookup.Where(g => 
                                                                    string.Equals(g.Genre, dto.Genre))
                                                                .FirstOrDefault();
                if (existingGenre == null)
                {
                    var newGenre = new EventGenreLookupTable(){Genre = dto.Genre};
                    _context.EventGenreLookup.Add(newGenre);
                    newEvent.Genre = newGenre;
                    newEvent.GenreId = newGenre.Id;
                }
                else 
                {
                    newEvent.Genre = existingGenre;
                    newEvent.GenreId = existingGenre.Id;
                }
                var existingUser = _context.Users.Where(u => u.UserId == newEvent.UserId).FirstOrDefault();
                existingUser.EventId = newEvent.EventId;
                existingUser.OwnedEvent = newEvent;
                newEvent.UserId = existingUser.UserId;
                newEvent.Owner = existingUser;
                _context.Events.Add(newEvent);
                _context.SaveChanges();
                return new ServiceResult<string>()
                {
                    Success = true,
                    Data = $"Event with name {newEvent.EventName} successfully created"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>()
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
    } 
    public ServiceResult<IEnumerable<EventDTO>> GetEvents()
    {
        try
        {
            return new ServiceResult<IEnumerable<EventDTO>>()
            {
                Success = true,
                Data = _context.Events.Include(e => e.Genre).Select(e => _dtoService.MapFromQuery(e))
            };
        }
        catch (Exception ex)
        {
            return new ServiceResult<IEnumerable<EventDTO>>()
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
