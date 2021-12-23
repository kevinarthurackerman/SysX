namespace Sysx.EntityFramework.Test.Identifiers;
using Assert = Xunit.Assert;

public class PropertyBuilderExtensionsTests
{
    [Fact]
    public async Task Should_Configure_Guid_Column_Types()
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