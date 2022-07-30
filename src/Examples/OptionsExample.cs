public class OptionsExample
{
	private static readonly SomeMethodOptions defaultOptions = SomeMethodOptions.Default;

	public void SomeMethod() => SomeMethod(in defaultOptions);

	public void SomeMethod(in SomeMethodOptions options)
	{
		options.Validate();

		// do something with options here
	}

	public record struct SomeMethodOptions
	{
		public static SomeMethodOptions Default => new()
		{
			Name = "Default Value",
			AltName = "Default Alt Value"
		};

		public string Name { get; set; }
		public string AltName { get; set; }

		public void Validate()
		{
			EnsureArg.IsNotNull(Name, nameof(Name));
			EnsureArg.IsNotEmptyOrWhiteSpace(Name, nameof(Name));
			EnsureArg.IsNotNull(AltName, nameof(AltName));
			EnsureArg.IsNotEmptyOrWhiteSpace(AltName, nameof(AltName));
		}
	}
}
