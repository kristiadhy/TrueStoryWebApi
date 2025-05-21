namespace TrueStoryWebApi.Responses;

public class ErrorResponse
{
  public int Status { get; init; }
  public required string Error { get; init; }
  public required string Message { get; init; }
  public required string Path { get; init; }
  public required string Timestamp { get; init; }
}