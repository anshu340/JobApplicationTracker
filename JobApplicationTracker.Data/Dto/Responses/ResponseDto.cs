namespace JobApplicationTracker.Data.Dtos.Responses;

public class ResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 200;
    public int Id { get; set; }  
}
