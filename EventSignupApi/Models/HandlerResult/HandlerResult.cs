
namespace EventSignupApi.Models.HandlerResult;

public class HandlerResult<T>
{
    public bool Success {get;init;}
    public T Data {get;init;}
    public string ErrorMessage {get; init;} = string.Empty;
}
