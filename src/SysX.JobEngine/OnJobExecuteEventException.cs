namespace SysX.JobEngine;

/// <summary>
/// An exception that wraps an exception thrown in the context of an on job execute event running.
/// </summary>
public class OnJobExecuteEventException : Exception
{
    public OnJobExecuteEventException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
