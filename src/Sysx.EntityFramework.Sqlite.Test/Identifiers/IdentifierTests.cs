namespace Sysx.EntityFramework.Test.Identifiers;
using Assert = Xunit.Assert;

public class IdentifierTests
{
    [Fact]
    public async Task Should_Persist_Guid_Values()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>();

        var testObject = new GuidPropertiesModel
        {
            Guid = Guid.NewGuid(),
            BinaryGuid = BinaryGuid.NewGuid(),
            StringGuid = StringGuid.NewGuid(),
            SqlServerGuid = SqlServerGuid.NewGuid()
        };

        dbContext.Add(testObject);

        await dbContext.SaveChangesAsync();

        var persistedTestObject = await dbContext.GuidPropertiesModels.SingleAsync();

        Assert.Equal(testObject.Guid, persistedTestObject.Guid);
        Assert.Equal(testObject.BinaryGuid, persistedTestObject.BinaryGuid);
        Assert.Equal(testObject.StringGuid, persistedTestObject.StringGuid);
        Assert.Equal(testObject.SqlServerGuid, persistedTestObject.SqlServerGuid);
    }

    [Fact]
    public async Task Should_Sort_Guids()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>();

        var testObjects = Enumerable.Range(1, 1000).Select(x => new GuidPropertiesModel
        {
            Guid = Guid.NewGuid()
        }).ToArray();

        dbContext.AddRange(testObjects);

        await dbContext.SaveChangesAsync();

        var testObjectsSortedInMemory = testObjects
            .OrderBy(x => x.Guid)
            .Select(x => x.Guid)
            .ToArray();

        var testObjectsSortedInDatabase = await dbContext.GuidPropertiesModels
            .OrderBy(x => x.Guid)
            .Select(x => x.Guid)
            .ToArrayAsync();

        Assert.Equal(testObjectsSortedInMemory.Length, testObjectsSortedInDatabase.Length);

        foreach (var (expected, actual) in testObjectsSortedInMemory.Zip(testObjectsSortedInDatabase))
        {
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public async Task Should_Sort_Binary_Guids()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>();

        var testObjects = Enumerable.Range(1, 1).Select(x => new GuidPropertiesModel
        {
            BinaryGuid = BinaryGuid.NewGuid()
        }).ToArray();

        dbContext.AddRange(testObjects);

        await dbContext.SaveChangesAsync();

        var testObjectsSortedInMemory = testObjects
            .OrderBy(x => x.BinaryGuid)
            .Select(x => x.BinaryGuid)
            .ToArray();

        var testObjectsSortedInDatabase = await dbContext.GuidPropertiesModels
            .OrderBy(x => x.BinaryGuid)
            .Select(x => x.BinaryGuid)
            .ToArrayAsync();

        Assert.Equal(testObjectsSortedInMemory.Length, testObjectsSortedInDatabase.Length);

        foreach (var (expected, actual) in testObjectsSortedInMemory.Zip(testObjectsSortedInDatabase))
        {
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public async Task Should_Sort_String_Guids()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>();

        var testObjects = Enumerable.Range(1, 1000).Select(x => new GuidPropertiesModel
        {
            StringGuid = StringGuid.NewGuid()
        }).ToArray();

        dbContext.AddRange(testObjects);

        await dbContext.SaveChangesAsync();

        var testObjectsSortedInMemory = testObjects
            .OrderBy(x => x.StringGuid)
            .Select(x => x.StringGuid)
            .ToArray();

        var testObjectsSortedInDatabase = await dbContext.GuidPropertiesModels
            .OrderBy(x => x.StringGuid)
            .Select(x => x.StringGuid)
            .ToArrayAsync();

        Assert.Equal(testObjectsSortedInMemory.Length, testObjectsSortedInDatabase.Length);

        foreach (var (expected, actual) in testObjectsSortedInMemory.Zip(testObjectsSortedInDatabase))
        {
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public async Task Should_Not_Sort_Sql_Server_Guids()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>();

        var testObjects = Enumerable.Range(1, 1000).Select(x => new GuidPropertiesModel
        {
            SqlServerGuid = SqlServerGuid.NewGuid()
        }).ToArray();

        dbContext.AddRange(testObjects);

        await dbContext.SaveChangesAsync();

        var testObjectsSortedInMemory = testObjects
            .OrderBy(x => x.SqlServerGuid)
            .Select(x => x.SqlServerGuid)
            .ToArray();

        var testObjectsSortedInDatabase = await dbContext.GuidPropertiesModels
            .OrderBy(x => x.SqlServerGuid)
            .Select(x => x.SqlServerGuid)
            .ToArrayAsync();

        Assert.Equal(testObjectsSortedInMemory.Length, testObjectsSortedInDatabase.Length);

        var equal = testObjectsSortedInMemory.Zip(testObjectsSortedInDatabase).All(x => x.First == x.Second);

        Assert.False(equal);
    }

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<GuidPropertiesModel> GuidPropertiesModels { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GuidPropertiesModel>()
                .ToTable("GuidProperties")
                .HasKey(x => x.Guid);

            modelBuilder.Entity<GuidPropertiesModel>()
                .Property(x => x.BinaryGuid)
                .IsBinaryGuid();

            modelBuilder.Entity<GuidPropertiesModel>()
                .Property(x => x.StringGuid)
                .IsStringGuid();

            modelBuilder.Entity<GuidPropertiesModel>()
                .Property(x => x.SqlServerGuid)
                .IsSqlServerGuid();
        }
    }

    public class GuidPropertiesModel
    {
        public Guid Guid { get; set; }
        public BinaryGuid BinaryGuid { get; set; }
        public StringGuid StringGuid { get; set; }
        public SqlServerGuid SqlServerGuid { get; set; }
    }
}