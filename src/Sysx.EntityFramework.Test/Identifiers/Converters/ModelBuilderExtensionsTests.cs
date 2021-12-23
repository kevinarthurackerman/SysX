namespace Sysx.EntityFramework.Test.Identifiers;
using Assert = Xunit.Assert;

public class ModelBuilderExtensionsTests
{
    [Fact]
    public async Task Should_Configure_Guid_Column_Types_On_SqlServer()
    {
        using var dbContext = SqlServerTestDbContextActivator.Create<TestDbContext>();

        var connection = dbContext.Database.GetDbConnection();

        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT [Guid], [BinaryGuid], [StringGuid], [SqlServerGuid] FROM [GuidProperties]";

        using var reader = command.ExecuteReader();

        Assert.Equal("uniqueidentifier", reader.GetDataTypeName(0));
        Assert.Equal("binary", reader.GetDataTypeName(1));
        Assert.Equal("char", reader.GetDataTypeName(2));
        Assert.Equal("uniqueidentifier", reader.GetDataTypeName(3));
    }

    [Fact]
    public async Task Should_Configure_Guid_Column_Types_On_Sqlite()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>();

        var connection = dbContext.Database.GetDbConnection();

        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT [Guid], [BinaryGuid], [StringGuid], [SqlServerGuid] FROM [GuidProperties]";

        using var reader = command.ExecuteReader();

        Assert.Equal("TEXT", reader.GetDataTypeName(0));
        Assert.Equal("BLOB", reader.GetDataTypeName(1));
        Assert.Equal("TEXT", reader.GetDataTypeName(2));
        Assert.Equal("TEXT", reader.GetDataTypeName(3));
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

            modelBuilder.RegisterGuidConversions();
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