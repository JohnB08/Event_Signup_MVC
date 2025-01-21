using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventSignupApi.Models;

public class Event
{
    public int EventId{get;set;}
    public required string EventName {get;set;} = string.Empty;
    public DateTime EventDate{get;set;}
    public bool Public {get;set;}
    public int GenreId {get;set;}
    [ForeignKey("Owner")]
    public int UserId {get;set;}
    public EventGenreLookupTable Genre{get;set;}
    public ICollection<User> SignUps {get;set;}
    public ICollection<User> Admins {get;set;}
    public User Owner {get;set;}
}
