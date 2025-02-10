using System;

namespace EventSignupApi.Models;

public class EventGenreLookupTable
{
    public int Id{get;init;}
    public required string Genre {get;init;}
    public ICollection<Event>? Events {get;init;} 
}
