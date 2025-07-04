
namespace JobApplicationTracker.Service.Exceptions;

public class ValidationException : Exception
{
    public List<string> Errors { get; } = new List<string>();

    public ValidationException() : base("One or more validatiaon exception occured.")
    {
        Errors = new List<string>();
    }

    public ValidationException(IEnumerable<string> errors)
    {
        Errors.AddRange(errors);
    }

    public ValidationException(string message, IEnumerable<string> errors) : base(message) 
    {
        Errors.AddRange(errors);
    }

    public ValidationException(string message, Exception innerException, IEnumerable<string> errors)
             : base(message, innerException)
    {
        Errors = new List<string>(errors);
    }
}
