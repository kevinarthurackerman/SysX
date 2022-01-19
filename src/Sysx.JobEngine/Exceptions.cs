namespace Sysx.JobEngine;

public class JobExecuteException : Exception
{
    public JobExecuteException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class OnJobExecuteEventException : Exception
{
    public OnJobExecuteEventException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}