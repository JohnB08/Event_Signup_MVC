using System;
using System.Text.Json.Serialization;

namespace EventSignupApi.Models.DTO;

public class EventDTO
{
    [JsonPropertyName("eventName")]
    public required string EventName {get;set;}
    [JsonPropertyName("date")]
    public string Date {get;set;}
    [JsonPropertyName("public")]
    public bool Public {get;set;}
    [JsonPropertyName("userId")]
    public int UserId {get;set;}
    [JsonPropertyName("genre")]
    public string Genre {get;set;}
}
