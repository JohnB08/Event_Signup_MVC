using System;

namespace EventSignupApi.Models.ServiceResult;

public class ServiceResult<T>
{
    public bool Success {get;set;}
    public string ErrorMessage {get;set;} = string.Empty;
    public T Data{get;set;}
}
