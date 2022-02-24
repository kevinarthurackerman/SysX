namespace SysX.JobEngine;

/// <summary>
/// An exception that wraps an exception thrown in the context of a job running.
/// </summary>
public class JobExecuteException : Exception
{
    public JobExecuteException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
