namespace Test_SysX.Enums.BaseEnumeration;
using Assert = Xunit.Assert;

public class SubclassTests
{
	public class Animal : BaseEnumeration<Animal, int>
	{
		public void Should_Contain_Own_Type()
		{
			Assert.Contains(Mammal.Lion, All);
			Assert.DoesNotContain(Reptile.Lizard, All);
		}

		public void Should_Contain_Both_Types()
		{
			Assert.Contains(Mammal.Lion, All);
			Assert.Contains(Reptile.Lizard, All);
		}

		private Animal(int value, string displayName) : base(value, displayName)
		{
		}

		public class Mammal : Animal
		{
			public static readonly Mammal Lion = new(1, nameof(Lion));
			public static readonly Mammal Tiger = new(1, nameof(Tiger));
			public static readonly Mammal Bear = new(1, nameof(Bear));

			private Mammal(int value, string displayName) : base(value, displayName)
			{
			}
		}

		public class Reptile : Animal
		{
			public static readonly Reptile Snake = new(1, nameof(Snake));
			public static readonly Reptile Lizard = new(1, nameof(Lizard));

			private Reptile(int value, string displayName) : base(value, displayName)
			{
			}
		}
	}
}
