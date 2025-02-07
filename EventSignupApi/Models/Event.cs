using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventSignupApi.Models;

public class Event
{
    public int EventId{get;set;}
    public required string EventName {get;set;} = string.Empty;
    public DateTime EventDate{get;set;}
    public bool Public {get;set;}
    [ForeignKey("Genre")]
    public int GenreId {get;set;}
    [ForeignKey("Owner")]
    public int UserId {get;set;}
    public double Lat {get;set;}
    public double Long {get;set;}
    public int MaxAttendees{get;set;}
    public required EventGenreLookupTable Genre{get;set;}
    public ICollection<User>? SignUps {get;set;}
    public ICollection<User>? Admins {get;set;}
    public required User Owner {get;set;}
}
