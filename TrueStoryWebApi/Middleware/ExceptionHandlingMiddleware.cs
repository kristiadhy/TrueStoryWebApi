using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using TrueStoryWebApi.Responses;

namespace TrueStoryWebApi.Middleware;

public class ExceptionHandlingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionHandlingMiddleware> _logger;
  private readonly JsonSerializerOptions _jsonOptions;

  public ExceptionHandlingMiddleware(
      RequestDelegate next,
      ILogger<ExceptionHandlingMiddleware> logger,
      IOptions<JsonSerializerOptions> jsonOptions)
  {
    _next = next;
    _logger = logger;
    _jsonOptions = jsonOptions.Value;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      await HandleExceptionAsync(context, ex);
    }
  }

  private async Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    _logger.LogError(exception, "Unhandled exception occurred");

    if (exception is HttpRequestException hre)
    {
      await HandleExternalApiErrorAsync(context, hre);
    }
    else
    {
      await HandleInternalErrorAsync(context, exception);
    }
  }

  private async Task HandleExternalApiErrorAsync(HttpContext context, HttpRequestException hre)
  {
    int statusCode = (int)HttpStatusCode.BadGateway;
    string content = "Failed to communicate with the external server.";
    context.Response.ContentType = "application/json";

    if (hre.StatusCode.HasValue)
    {
      statusCode = (int)hre.StatusCode.Value;
    }

    var errorResponse = new ErrorResponse
    {
      Status = statusCode,
      Error = GetReasonPhrase(statusCode),
      Message = content,
      Path = context.Request.Path,
      Timestamp = DateTime.UtcNow.ToString("o")
    };

    context.Response.StatusCode = statusCode;

    var json = JsonSerializer.Serialize(errorResponse, _jsonOptions);
    await context.Response.WriteAsync(json);
  }

  private async Task HandleInternalErrorAsync(HttpContext context, Exception exception)
  {
    var errorResponse = exception switch
    {
      UnauthorizedAccessException => CreateErrorResponse((int)HttpStatusCode.Unauthorized, "Unauthorized", "Unauthorized access to the resource.", context.Request.Path),
      ArgumentException ex => CreateErrorResponse((int)HttpStatusCode.BadRequest, "Bad Request", ex.Message, context.Request.Path),
      KeyNotFoundException => CreateErrorResponse((int)HttpStatusCode.NotFound, "Not Found", "The specified resource was not found.", context.Request.Path),
      _ => CreateErrorResponse((int)HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred.", context.Request.Path)
    };

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = errorResponse.Status;

    var json = JsonSerializer.Serialize(errorResponse, _jsonOptions);
    await context.Response.WriteAsync(json);
  }

  private ErrorResponse CreateErrorResponse(int statusCode, string error, string message, string path)
  {
    return new ErrorResponse
    {
      Status = statusCode,
      Error = error,
      Message = message,
      Path = path,
      Timestamp = DateTime.UtcNow.ToString("o")
    };
  }

  private string GetReasonPhrase(int statusCode) => statusCode switch
  {
    400 => "Bad Request",
    401 => "Unauthorized",
    403 => "Forbidden",
    404 => "Not Found",
    405 => "Method Not Allowed",
    429 => "Too Many Requests",
    502 => "Bad Gateway",
    503 => "Service Unavailable",
    500 => "Internal Server Error",
    _ => Enum.IsDefined(typeof(HttpStatusCode), statusCode)
         ? ((HttpStatusCode)statusCode).ToString()
         : "Unknown Error"
  };
}
