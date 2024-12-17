using System.Net;
using System.Text.Json;

namespace Rise.Shared.Helpers;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }

    public ErrorDetails()
    {
    }
    public ErrorDetails(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        Message = message;
        StatusCode = (int)statusCode;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

}
