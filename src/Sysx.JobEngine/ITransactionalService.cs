namespace Sysx.JobEngine;

public interface ITransactionalService
{
    public void EnlistTransaction(Transaction transaction);
}
