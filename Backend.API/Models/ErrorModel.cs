namespace Backend.API.Models;

public class ErrorModel
{
    public int StatusCode { get; set; } = int.MinValue;
    public string Message { get; set; } = string.Empty;
}