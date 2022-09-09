namespace SysX.EnsureThat;

public static class ParamExtensions
{
	/// <summary>
	/// Ensures that the <see cref="IDisposable"/> <paramref name="param"/> value is not disposed.
	/// </summary>
	public static void IsNotDisposed<T>(this Param<T> param, bool isDisposed)
		where T : IDisposable
	{
		EnsureArg.HasValue(param.Value, nameof(param));

		if (!isDisposed) return;

		throw ThrowException(
			param,
			$"Disposed object {param.Name} can no longer be used.",
			message => new ObjectDisposedException(param.Value.GetType().FullName, message));
	}

	/// <summary>
	/// Ensures that the <see cref="IAsyncDisposable"/> <paramref name="param"/> value is not disposed.
	/// </summary>
	public static void IsNotAsyncDisposed<T>(this Param<T> param, bool isDisposed)
		where T : IAsyncDisposable
	{
		EnsureArg.HasValue(param.Value, nameof(param));

		if (!isDisposed) return;

		throw ThrowException(
			param,
			$"Disposed object {param.Name} can no longer be used.",
			message => new ObjectDisposedException(param.Value.GetType().FullName, message));
	}

	/// <summary>
	/// Ensure that the <see cref="IEnumerable"/> <paramref name="param"/> value does not contain a <see langword="null"/> value.
	/// </summary>
	public static void DoesNotContainNull<TEnumerable>(this Param<TEnumerable> param)
		where TEnumerable : IEnumerable
	{
		if (param.Value == null) return;

		foreach (var value in param.Value)
		{
			if (value == null)
			{
				throw ThrowException(
					param,
					$"Value '{param.Name}' must not contain null values.",
					message => new ArgumentNullException(param.Name, message));
			}
		}
	}

	/// <summary>
	/// Ensure that the <see cref="IEnumerable"/> <paramref name="param"/> value does not contain a given value.
	/// </summary>
	public static void DoesNotContain<T, TEnumerable>(this Param<TEnumerable> param, T value)
		where TEnumerable : IEnumerable
	{
		if (param.Value == null) return;

		if (value == null)
		{
			foreach (var val in param.Value)
				if (val == null) Throw();
		}
		else
		{
			foreach (var val in param.Value)
				if (val is T tVal && tVal.Equals(value)) Throw();
		}

		void Throw()
		{
			var commaSeparatedValues = string.Join(", ", param.Value);

			throw ThrowException(
				param,
				$"Value '{param.Name}' '[{commaSeparatedValues}]' must not contain a value of {value}.",
				message => new ArgumentException(param.Name, message));
		}
	}

	/// <summary>
	/// Ensure that the <see cref="string"/> <paramref name="param"/> does not contain a given <see cref="char"/>.
	/// </summary>
	public static void DoesNotContain(this StringParam param, char value)
	{
		if (param.Value == null) return;

		if (param.Value.All(x => x != value)) return;

		throw ThrowException(
			param,
			$"Value '{param.Name}' '{param.Value}' must not contain char {value}.",
			message => new ArgumentException(param.Name, message));
	}

	/// <summary>
	/// Ensure that the <paramref name="param"/> value in not contained in the given <see cref="IEnumerable"/>.
	/// </summary>
	public static void IsNotContainedIn<T, TEnumerable>(this Param<T> param, TEnumerable value)
		where TEnumerable : IEnumerable<T>
	{
		if (value == null) return;

		if (param.Value == null)
		{
			foreach (var val in value)
				if (val == null) Throw();
		}
		else
		{
			foreach (var val in value)
				if (val is T tVal && tVal.Equals(param.Value)) Throw();
		}

		void Throw()
		{
			var commaSeparatedValues = string.Join(", ", value);

			throw ThrowException(
				param,
				$"Value '{param.Name}' '{param.Value}' must not be contained in '[{commaSeparatedValues}]'.",
				message => new ArgumentException(param.Name, message));
		}
	}

	private static Exception ThrowException<T>(
		Param<T> param, string defaultMessage, DefaultExceptionFactory defaultExceptionFactory)
	{
		var options = param.OptsFn?.Invoke(new EnsureOptions()) ?? new EnsureOptions();

		var message = options.CustomMessage ?? defaultMessage;

		var exception = options.CustomExceptionFactory?.Invoke(message, param.Name)
			?? options.CustomException
			?? defaultExceptionFactory.Invoke(message);

		return exception;
	}

	private static Exception ThrowException(
		StringParam param, string defaultMessage, DefaultExceptionFactory defaultExceptionFactory)
	{
		var options = param.OptsFn?.Invoke(new EnsureOptions()) ?? new EnsureOptions();

		var message = options.CustomMessage ?? defaultMessage;

		var exception = options.CustomExceptionFactory?.Invoke(message, param.Name)
			?? options.CustomException
			?? defaultExceptionFactory.Invoke(message);

		return exception;
	}

	private delegate Exception DefaultExceptionFactory(string message);
}
