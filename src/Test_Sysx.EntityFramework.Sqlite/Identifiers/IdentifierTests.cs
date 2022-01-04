namespace Test_Sysx.EntityFramework.Sqlite.Identifiers;
using Assert = Assert;

public class IdentifierTests
{
    [Fact]
    public async Task Should_Configure_Guid_Column_Types()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseSequentialGuids());

        var connection = dbContext.Database.GetDbConnection();

        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = $@"
SELECT
    [{nameof(GuidPropertiesModel.Guid)}],
    [{nameof(GuidPropertiesModel.BinaryGuid)}],
    [{nameof(GuidPropertiesModel.StringGuid)}],
    [{nameof(GuidPropertiesModel.SqlServerGuid)}],
    [{nameof(GuidPropertiesModel.NullableGuid)}],
    [{nameof(GuidPropertiesModel.NullableBinaryGuid)}],
    [{nameof(GuidPropertiesModel.NullableStringGuid)}],
    [{nameof(GuidPropertiesModel.NullableSqlServerGuid)}]
FROM [GuidProperties]";

        using var reader = command.ExecuteReader();

        var ordinal = 0;
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("BLOB", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("BLOB", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
    }

    [Fact]
    public async Task Should_Persist_Guid_Values()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseSequentialGuids());

        var testObject = new GuidPropertiesModel
        {
            Guid = Guid.NewGuid(),
            BinaryGuid = BinaryGuid.NewGuid(),
            StringGuid = StringGuid.NewGuid(),
            SqlServerGuid = SqlServerGuid.NewGuid(),
            NullableGuid = Guid.NewGuid(),
            NullableBinaryGuid = BinaryGuid.NewGuid(),
            NullableStringGuid = StringGuid.NewGuid(),
            NullableSqlServerGuid = SqlServerGuid.NewGuid()
        };

        dbContext.Add(testObject);

        await dbContext.SaveChangesAsync();

        var persistedTestObject = await dbContext.GuidPropertiesModels.SingleAsync();

        Assert.Equal(testObject.Guid, persistedTestObject.Guid);
        Assert.Equal(testObject.BinaryGuid, persistedTestObject.BinaryGuid);
        Assert.Equal(testObject.StringGuid, persistedTestObject.StringGuid);
        Assert.Equal(testObject.SqlServerGuid, persistedTestObject.SqlServerGuid);
        Assert.Equal(testObject.NullableGuid, persistedTestObject.NullableGuid);
        Assert.Equal(testObject.NullableBinaryGuid, persistedTestObject.NullableBinaryGuid);
        Assert.Equal(testObject.NullableStringGuid, persistedTestObject.NullableStringGuid);
        Assert.Equal(testObject.NullableSqlServerGuid, persistedTestObject.NullableSqlServerGuid);
    }

    [Fact]
    public async Task Should_Persist_Default_Guid_Values()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseSequentialGuids());

        var testObject = new GuidPropertiesModel
        {
            Guid = Guid.NewGuid(),
            BinaryGuid = default,
            StringGuid = default,
            SqlServerGuid = default,
            NullableGuid = default,
            NullableBinaryGuid = default,
            NullableStringGuid = default,
            NullableSqlServerGuid = default
        };

        dbContext.Add(testObject);

        await dbContext.SaveChangesAsync();

        var persistedTestObject = await dbContext.GuidPropertiesModels.SingleAsync();

        Assert.Equal(testObject.Guid, persistedTestObject.Guid);
        Assert.Equal(testObject.BinaryGuid, persistedTestObject.BinaryGuid);
        Assert.Equal(testObject.StringGuid, persistedTestObject.StringGuid);
        Assert.Equal(testObject.SqlServerGuid, persistedTestObject.SqlServerGuid);
        Assert.Equal(testObject.NullableGuid, persistedTestObject.NullableGuid);
        Assert.Equal(testObject.NullableBinaryGuid, persistedTestObject.NullableBinaryGuid);
        Assert.Equal(testObject.NullableStringGuid, persistedTestObject.NullableStringGuid);
        Assert.Equal(testObject.NullableSqlServerGuid, persistedTestObject.NullableSqlServerGuid);
    }

    [Fact]
    public async Task Should_Sort_Guids()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseSequentialGuids());

        var testObjects = Enumerable.Range(1, 1000).Select(x => new GuidPropertiesModel
        {
            Guid = Guid.NewGuid()
        })
        .Append(new GuidPropertiesModel())
        .ToArray();

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
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseSequentialGuids());

        var testObjects = Enumerable.Range(1, 1).Select(x => new GuidPropertiesModel
        {
            BinaryGuid = BinaryGuid.NewGuid()
        })
        .Append(new GuidPropertiesModel())
        .ToArray();

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
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseSequentialGuids());

        var testObjects = Enumerable.Range(1, 1000).Select(x => new GuidPropertiesModel
        {
            StringGuid = StringGuid.NewGuid()
        })
        .Append(new GuidPropertiesModel())
        .ToArray();

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
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseSequentialGuids());

        var testObjects = Enumerable.Range(1, 1000).Select(x => new GuidPropertiesModel
        {
            SqlServerGuid = SqlServerGuid.NewGuid()
        })
        .Append(new GuidPropertiesModel())
        .ToArray();

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

    [Fact]
    public async Task Should_Sort_Nullable_Guids()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseSequentialGuids());

        var testObjects = Enumerable.Range(1, 1000).Select(x => new GuidPropertiesModel
        {
            NullableGuid = Guid.NewGuid()
        })
        .Append(new GuidPropertiesModel())
        .ToArray();

        dbContext.AddRange(testObjects);

        await dbContext.SaveChangesAsync();

        var testObjectsSortedInMemory = testObjects
            .OrderBy(x => x.NullableGuid)
            .Select(x => x.NullableGuid)
            .ToArray();

        var testObjectsSortedInDatabase = await dbContext.GuidPropertiesModels
            .OrderBy(x => x.NullableGuid)
            .Select(x => x.NullableGuid)
            .ToArrayAsync();

        Assert.Equal(testObjectsSortedInMemory.Length, testObjectsSortedInDatabase.Length);

        foreach (var (expected, actual) in testObjectsSortedInMemory.Zip(testObjectsSortedInDatabase))
        {
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public async Task Should_Sort_Nullable_Binary_Guids()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseSequentialGuids());

        var testObjects = Enumerable.Range(1, 1).Select(x => new GuidPropertiesModel
        {
            NullableBinaryGuid = BinaryGuid.NewGuid()
        })
        .Append(new GuidPropertiesModel())
        .ToArray();

        dbContext.AddRange(testObjects);

        await dbContext.SaveChangesAsync();

        var testObjectsSortedInMemory = testObjects
            .OrderBy(x => x.NullableBinaryGuid)
            .Select(x => x.NullableBinaryGuid)
            .ToArray();

        var testObjectsSortedInDatabase = await dbContext.GuidPropertiesModels
            .OrderBy(x => x.NullableBinaryGuid)
            .Select(x => x.NullableBinaryGuid)
            .ToArrayAsync();

        Assert.Equal(testObjectsSortedInMemory.Length, testObjectsSortedInDatabase.Length);

        foreach (var (expected, actual) in testObjectsSortedInMemory.Zip(testObjectsSortedInDatabase))
        {
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public async Task Should_Sort_Nullable_String_Guids()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseSequentialGuids());

        var testObjects = Enumerable.Range(1, 1000).Select(x => new GuidPropertiesModel
        {
            NullableStringGuid = StringGuid.NewGuid()
        })
        .Append(new GuidPropertiesModel())
        .ToArray();

        dbContext.AddRange(testObjects);

        await dbContext.SaveChangesAsync();

        var testObjectsSortedInMemory = testObjects
            .OrderBy(x => x.NullableStringGuid)
            .Select(x => x.NullableStringGuid)
            .ToArray();

        var testObjectsSortedInDatabase = await dbContext.GuidPropertiesModels
            .OrderBy(x => x.NullableStringGuid)
            .Select(x => x.NullableStringGuid)
            .ToArrayAsync();

        Assert.Equal(testObjectsSortedInMemory.Length, testObjectsSortedInDatabase.Length);

        foreach (var (expected, actual) in testObjectsSortedInMemory.Zip(testObjectsSortedInDatabase))
        {
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public async Task Should_Not_Sort_Nullable_Sql_Server_Guids()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseSequentialGuids());

        var testObjects = Enumerable.Range(1, 1000).Select(x => new GuidPropertiesModel
        {
            NullableSqlServerGuid = SqlServerGuid.NewGuid()
        })
        .Append(new GuidPropertiesModel())
        .ToArray();

        dbContext.AddRange(testObjects);

        await dbContext.SaveChangesAsync();

        var testObjectsSortedInMemory = testObjects
            .OrderBy(x => x.NullableSqlServerGuid)
            .Select(x => x.NullableSqlServerGuid)
            .ToArray();

        var testObjectsSortedInDatabase = await dbContext.GuidPropertiesModels
            .OrderBy(x => x.NullableSqlServerGuid)
            .Select(x => x.NullableSqlServerGuid)
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
        }
    }

    public class GuidPropertiesModel
    {
        public Guid Guid { get; set; }
        public BinaryGuid BinaryGuid { get; set; }
        public StringGuid StringGuid { get; set; }
        public SqlServerGuid SqlServerGuid { get; set; }
        public Guid? NullableGuid { get; set; }
        public BinaryGuid? NullableBinaryGuid { get; set; }
        public StringGuid? NullableStringGuid { get; set; }
        public SqlServerGuid? NullableSqlServerGuid { get; set; }
    }
}