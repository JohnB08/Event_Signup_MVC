using System;
using System.Text.Json.Serialization;

namespace EventSignupApi.Models.DTO;

public class EventDto
{
    [JsonPropertyName("eventName")]
    public required string EventName {get;set;}
    [JsonPropertyName("date")]
    public required string Date {get;set;}
    [JsonPropertyName("public")]
    public bool Public {get;set;}
    [JsonPropertyName("canEdit")]
    public bool CanEdit {get;set;} = false;
    [JsonPropertyName("isSubscriber")]
    public bool IsSubscriber {get;set;} = false;
    [JsonPropertyName("genre")]
    public required string Genre {get;set;}
    [JsonPropertyName("maxAttendees")]
    public int MaxAttendees{get;set;}
    [JsonPropertyName("latLong")]
    public required double[] LatLong {get;set;}
    public int? Id {get;set;}
}
