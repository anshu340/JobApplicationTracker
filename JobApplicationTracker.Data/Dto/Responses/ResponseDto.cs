namespace JobApplicationTracker.Data.Dtos.Responses;

public class ResponseDto
{
    public int Id { get; set; }
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
}