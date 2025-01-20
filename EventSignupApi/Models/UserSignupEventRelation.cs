using System;

namespace EventSignupApi.Models;

public class UserSignupEventRelation
{
    public int Id{get;set;}
    public int UserId{get;set;}
    public int EventId{get;set;}
}
