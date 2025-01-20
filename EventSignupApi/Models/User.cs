using System;

namespace EventSignupApi.Models;

public class User
{
    public int UserId{get;set;}
    public required string UserName{get;set;} = string.Empty;
    public required string Hash{get;set;} = string.Empty;
}
