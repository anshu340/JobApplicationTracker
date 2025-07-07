namespace JobApplicationTracker.Api.ApiResponses;

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; } 
}