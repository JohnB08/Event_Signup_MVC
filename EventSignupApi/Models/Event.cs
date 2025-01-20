using System;

namespace EventSignupApi.Models;

public class Event
{
    public int EventId{get;set;}
    public required string EventName {get;set;} = string.Empty;
    public DateTime EventDate{get;set;}
    public bool Public {get;set;}
    public int GenreId {get;set;}
}
