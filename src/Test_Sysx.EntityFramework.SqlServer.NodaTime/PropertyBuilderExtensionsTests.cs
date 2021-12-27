namespace Test_Sysx.EntityFramework.SqlServer.Identifiers;
using Assert = Assert;

public class PropertyBuilderExtensionsTests
{
    [Fact]
    public async Task Should_Configure_Column_Types()
    {
        using var dbContext = SqlServerTestDbContextActivator.Create<TestDbContext>();

        var connection = dbContext.Database.GetDbConnection();

        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = $@"
SELECT
    [{nameof(NodaTypesModel.Guid)}],
    [{nameof(NodaTypesModel.Duration)}],
    [{nameof(NodaTypesModel.Instant)}],
    [{nameof(NodaTypesModel.LocalDateTime)}],
    [{nameof(NodaTypesModel.LocalDate)}],
    [{nameof(NodaTypesModel.LocalTime)}],
    [{nameof(NodaTypesModel.OffsetDateTime)}],
    [{nameof(NodaTypesModel.Offset)}],
    [{nameof(NodaTypesModel.ZonedDateTime)}],
    [{nameof(NodaTypesModel.NullableDuration)}],
    [{nameof(NodaTypesModel.NullableInstant)}],
    [{nameof(NodaTypesModel.NullableLocalDateTime)}],
    [{nameof(NodaTypesModel.NullableLocalDate)}],
    [{nameof(NodaTypesModel.NullableLocalTime)}],
    [{nameof(NodaTypesModel.NullableOffsetDateTime)}],
    [{nameof(NodaTypesModel.NullableOffset)}],
    [{nameof(NodaTypesModel.NullableZonedDateTime)}]
FROM [NodaTypesModels]";

        using var reader = command.ExecuteReader();

        var ordinal = 0;
        Assert.Equal("uniqueidentifier", reader.GetDataTypeName(ordinal++));
        Assert.Equal("time", reader.GetDataTypeName(ordinal++));
        Assert.Equal("datetime2", reader.GetDataTypeName(ordinal++));
        Assert.Equal("datetime2", reader.GetDataTypeName(ordinal++));
        Assert.Equal("date", reader.GetDataTypeName(ordinal++));
        Assert.Equal("time", reader.GetDataTypeName(ordinal++));
        Assert.Equal("datetimeoffset", reader.GetDataTypeName(ordinal++));
        Assert.Equal("time", reader.GetDataTypeName(ordinal++));
        Assert.Equal("datetimeoffset", reader.GetDataTypeName(ordinal++));
        Assert.Equal("time", reader.GetDataTypeName(ordinal++));
        Assert.Equal("datetime2", reader.GetDataTypeName(ordinal++));
        Assert.Equal("datetime2", reader.GetDataTypeName(ordinal++));
        Assert.Equal("date", reader.GetDataTypeName(ordinal++));
        Assert.Equal("time", reader.GetDataTypeName(ordinal++));
        Assert.Equal("datetimeoffset", reader.GetDataTypeName(ordinal++));
        Assert.Equal("time", reader.GetDataTypeName(ordinal++));
        Assert.Equal("datetimeoffset", reader.GetDataTypeName(ordinal++));
    }

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<NodaTypesModel> NodaTypesModels { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NodaTypesModel>()
                .ToTable("NodaTypesModels")
                .HasKey(x => x.Guid);

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.Duration)
                .IsDuration();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.Instant)
                .IsInstant();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.LocalDateTime)
                .IsLocalDateTime();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.LocalDate)
                .IsLocalDate();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.LocalTime)
                .IsLocalTime();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.OffsetDateTime)
                .IsOffsetDateTime();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.Offset)
                .IsOffset();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.ZonedDateTime)
                .IsZonedDateTime();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.NullableDuration)
                .IsDuration();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.NullableInstant)
                .IsInstant();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.NullableLocalDateTime)
                .IsLocalDateTime();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.NullableLocalDate)
                .IsLocalDate();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.NullableLocalTime)
                .IsLocalTime();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.NullableOffsetDateTime)
                .IsOffsetDateTime();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.NullableOffset)
                .IsOffset();

            modelBuilder.Entity<NodaTypesModel>()
                .Property(x => x.NullableZonedDateTime)
                .IsZonedDateTime();
        }
    }

    public class NodaTypesModel
    {
        public Guid Guid { get; set; }
        public Duration Duration { get; set; }
        public Instant Instant { get; set; }
        public LocalDateTime LocalDateTime { get; set; }
        public LocalDate LocalDate { get; set; }
        public LocalTime LocalTime { get; set; }
        public OffsetDateTime OffsetDateTime { get; set; }
        public Offset Offset { get; set; }
        public ZonedDateTime ZonedDateTime { get; set; }
        public Duration? NullableDuration { get; set; }
        public Instant? NullableInstant { get; set; }
        public LocalDateTime? NullableLocalDateTime { get; set; }
        public LocalDate? NullableLocalDate { get; set; }
        public LocalTime? NullableLocalTime { get; set; }
        public OffsetDateTime? NullableOffsetDateTime { get; set; }
        public Offset? NullableOffset { get; set; }
        public ZonedDateTime? NullableZonedDateTime { get; set; }
    }
}