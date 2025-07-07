namespace JobApplicationTracker.Service.Exceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string message) : base(message) { }
    public DuplicateEmailException(string message, Exception innerException) : base(message, innerException) { }
}