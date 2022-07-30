namespace SysX.JobEngine;

/// <inheritdoc cref="Queue" />
public interface IQueue
{
	/// <summary>
	/// Adds a job to be ran on this queue after all previously submitted jobs have completed.
	/// Jobs added in the scope of a transaction will be batched together and added at the
	/// end of the queue when the transaction is completed.
	/// </summary>
	public void SubmitJob<TJob>(TJob data)
		where TJob : IJob;

	/// <summary>
	/// Adds a job to be ran on this queue before all other jobs. 
	/// Child jobs must be added in the scope of a transaction will be batched
	/// and submitted together when the transaction is completed.
	/// </summary>
	public void SubmitChildJob<TJob>(TJob jobData)
		where TJob : IJob;
}
