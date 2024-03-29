﻿namespace SysX.JobEngine;

/// <summary>
/// A service used to get the queue which is executing the current job.
/// </summary>
public class QueueContext : IQueueContext
{
	/// <summary>
	/// The queue executing the current job.
	/// </summary>
	public IQueue Current { get; private set; } = null!;

	void IQueueContext.SetCurrent(IQueue queue)
	{
		EnsureArg.IsNotNull(queue, nameof(queue));

		Current = queue;
	}
}
