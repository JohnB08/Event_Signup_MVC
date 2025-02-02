using System;
using System.Text.Json.Serialization;

namespace EventSignupApi.Models.DTO;

public class EventDTO
{
    [JsonPropertyName("eventName")]
    public required string EventName {get;set;}
    [JsonPropertyName("date")]
    public required string Date {get;set;}
    [JsonPropertyName("public")]
    public bool Public {get;set;}
    [JsonPropertyName("canEdit")]
    public bool CanEdit {get;set;} = false;
    [JsonPropertyName("genre")]
    public required string Genre {get;set;}
    [JsonPropertyName("maxAttendees")]
    public int MaxAttendees{get;set;}
}
