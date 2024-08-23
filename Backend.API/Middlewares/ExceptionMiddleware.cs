using System.Net;
using System.Text.Json;
using Backend.API.Models;
using Backend.Application.Exceptions;

namespace Backend.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionMessageAsync(context, ex).ConfigureAwait(false);
        }
    }

    private Task HandleExceptionMessageAsync(HttpContext context, Exception exception)
    {
        int statusCode = 0;
        var message = string.Empty;

        switch (exception)
        {
            case BadRequestException:
                statusCode = (int)HttpStatusCode.BadRequest;
                break;
            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "An error occurred on the server side";
                break;
        }

        var result = JsonSerializer.Serialize(new ErrorModel
        {
            StatusCode = statusCode,
            Message = message == string.Empty ? exception.Message : message
        });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        Console.WriteLine($"Message: {exception.Message}\n InnerMessage: {exception.InnerException}");
            
        return context.Response.WriteAsync(result);
    }
}