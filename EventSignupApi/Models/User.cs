using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventSignupApi.Models;

public class User
{
    public int UserId{get;set;}
    public required string UserName{get;set;} = string.Empty;
    public required string Hash{get;set;} = string.Empty;
    [ForeignKey("OwnedEvent")]
    public int EventId {get;set;}
    public ICollection<Event> SignUpEvents {get;set;}
    public ICollection<Event> AdminEvents {get;set;}
    public Event OwnedEvent{get;set;}
}
