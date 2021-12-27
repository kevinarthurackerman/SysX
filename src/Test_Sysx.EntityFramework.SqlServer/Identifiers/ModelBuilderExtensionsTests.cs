namespace Test_Sysx.EntityFramework.SqlServer.Identifiers;
using Assert = Assert;

public class ModelBuilderExtensionsTests
{
    [Fact]
    public async Task Should_Configure_Guid_Column_Types()
    {
        using var dbContext = SqlServerTestDbContextActivator.Create<TestDbContext>();

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
        Assert.Equal("uniqueidentifier", reader.GetDataTypeName(ordinal++));
        Assert.Equal("binary", reader.GetDataTypeName(ordinal++));
        Assert.Equal("char", reader.GetDataTypeName(ordinal++));
        Assert.Equal("uniqueidentifier", reader.GetDataTypeName(ordinal++));
        Assert.Equal("uniqueidentifier", reader.GetDataTypeName(ordinal++));
        Assert.Equal("binary", reader.GetDataTypeName(ordinal++));
        Assert.Equal("char", reader.GetDataTypeName(ordinal++));
        Assert.Equal("uniqueidentifier", reader.GetDataTypeName(ordinal++));
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

            modelBuilder.RegisterSequentialGuidConversions();
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