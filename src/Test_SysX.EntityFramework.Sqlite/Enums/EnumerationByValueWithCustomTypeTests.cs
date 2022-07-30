namespace Test_Sysx.EntityFramework.Sqlite.Enums;

public class EnumerationByValueWithCustomTypeTests
{
	[Fact]
	public async Task Should_Configure_Enumeration_Column_Types()
	{
		using var dbContext = SqliteTestDbContextActivator
			.Create<TestDbContext>(x => x.Provider.UseEnumerationsByValue().UseIdentifiers());

		var connection = dbContext.Database.GetDbConnection();

		await connection.OpenAsync();

		using var command = connection.CreateCommand();
		command.CommandText = $@"
SELECT
	[{nameof(EnumerationPropertiesModel.Id)}],
	[{nameof(EnumerationPropertiesModel.Null)}],
	[{nameof(EnumerationPropertiesModel.Lion)}],
	[{nameof(EnumerationPropertiesModel.Snake)}],
	[{nameof(EnumerationPropertiesModel.LionAnimal)}],
	[{nameof(EnumerationPropertiesModel.SnakeAnimal)}]
FROM [EnumerationProperties]";

		using var reader = command.ExecuteReader();

		var ordinal = 0;
		Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
		Assert.Equal("BLOB", reader.GetDataTypeName(ordinal++));
		Assert.Equal("BLOB", reader.GetDataTypeName(ordinal++));
		Assert.Equal("BLOB", reader.GetDataTypeName(ordinal++));
		Assert.Equal("BLOB", reader.GetDataTypeName(ordinal++));
		Assert.Equal("BLOB", reader.GetDataTypeName(ordinal++));
	}

	[Fact]
	public async Task Should_Persist_Enumeration_Values()
	{
		using var dbContext = SqliteTestDbContextActivator
			.Create<TestDbContext>(x => x.Provider.UseEnumerationsByValue().UseIdentifiers());

		var testObject = new EnumerationPropertiesModel
		{
			Id = Guid.NewGuid(),
			Null = Animal.Null,
			Lion = Mammal.Lion,
			Snake = Reptile.Snake,
			LionAnimal = Mammal.Lion,
			SnakeAnimal = Reptile.Snake
		};

		dbContext.Add(testObject);

		await dbContext.SaveChangesAsync();

		var persistedTestObject = await dbContext.EnumerationPropertiesModels.SingleAsync();

		Assert.Equal(testObject.Id, persistedTestObject.Id);
		Assert.Equal(testObject.Null, persistedTestObject.Null);
		Assert.Equal(testObject.Lion, persistedTestObject.Lion);
		Assert.Equal(testObject.Snake, persistedTestObject.Snake);
		Assert.Equal(testObject.LionAnimal, persistedTestObject.LionAnimal);
		Assert.Equal(testObject.SnakeAnimal, persistedTestObject.SnakeAnimal);
	}

	[Fact]
	public async Task Should_Persist_Default_Enumeration_Values()
	{
		using var dbContext = SqliteTestDbContextActivator
			.Create<TestDbContext>(x => x.Provider.UseEnumerationsByValue().UseIdentifiers());

		var testObject = new EnumerationPropertiesModel
		{
			Id = Guid.NewGuid(),
			Null = default,
			Lion = default,
			Snake = default,
			LionAnimal = default,
			SnakeAnimal = default
		};

		dbContext.Add(testObject);

		await dbContext.SaveChangesAsync();

		var persistedTestObject = await dbContext.EnumerationPropertiesModels.SingleAsync();

		Assert.Equal(testObject.Id, persistedTestObject.Id);
		Assert.Equal(testObject.Null, persistedTestObject.Null);
		Assert.Equal(testObject.Lion, persistedTestObject.Lion);
		Assert.Equal(testObject.Snake, persistedTestObject.Snake);
		Assert.Equal(testObject.LionAnimal, persistedTestObject.LionAnimal);
		Assert.Equal(testObject.SnakeAnimal, persistedTestObject.SnakeAnimal);
	}

	public class TestDbContext : DbContext
	{
		public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

		public DbSet<EnumerationPropertiesModel> EnumerationPropertiesModels { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<EnumerationPropertiesModel>()
				.ToTable("EnumerationProperties")
				.HasKey(x => x.Id);
		}
	}

	public class EnumerationPropertiesModel
	{
		public Guid Id { get; set; }
		public Animal? Null { get; set; }
		public Mammal? Lion { get; set; }
		public Reptile? Snake { get; set; }
		public Animal? LionAnimal { get; set; }
		public Animal? SnakeAnimal { get; set; }
	}

	public class Animal : BaseEnumeration<Animal, BinaryGuid>
	{
		public static readonly Animal Null = new(BinaryGuid.NewGuid(), "Null");

		protected Animal(BinaryGuid value, string displayName) : base(value, displayName) { }
	}

	public class Mammal : Animal
	{
		public static readonly Mammal Lion = new(BinaryGuid.NewGuid(), "Lion");
		public static readonly Mammal Tiger = new(BinaryGuid.NewGuid(), "Tiger");
		public static readonly Mammal Bear = new(BinaryGuid.NewGuid(), "Bear");

		protected Mammal(BinaryGuid value, string displayName) : base(value, displayName) { }
	}

	public class Reptile : Animal
	{
		public static readonly Reptile Snake = new(BinaryGuid.NewGuid(), "Snake");
		public static readonly Reptile Turtle = new(BinaryGuid.NewGuid(), "Turtle");

		protected Reptile(BinaryGuid value, string displayName) : base(value, displayName) { }
	}
}