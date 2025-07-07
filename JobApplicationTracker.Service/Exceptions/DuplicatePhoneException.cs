namespace JobApplicationTracker.Service.Exceptions;

public class DuplicatePhoneNumberException : Exception
{
    public DuplicatePhoneNumberException(string message) : base(message) { }
    public DuplicatePhoneNumberException(string message, Exception innerException) : base(message, innerException) { }
}