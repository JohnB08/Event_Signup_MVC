using System;

namespace EventSignupApi.Models;

public class EventGenreLookupTable
{
    public int Id{get;set;}
    public required string Genre {get;set;}
    public ICollection<Event> Events {get;set;} 
}
