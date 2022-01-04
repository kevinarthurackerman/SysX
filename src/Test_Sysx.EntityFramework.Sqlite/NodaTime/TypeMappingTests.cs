namespace Test_Sysx.EntityFramework.Sqlite.NodaTime;
using Assert = Assert;

public class TypeMappingTests
{
    [Fact]
    public async Task Should_Configure_Column_Types()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseNodaTime());

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
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("INTEGER", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("INTEGER", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("INTEGER", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
        Assert.Equal("INTEGER", reader.GetDataTypeName(ordinal++));
        Assert.Equal("TEXT", reader.GetDataTypeName(ordinal++));
    }

    [Fact]
    public async Task Should_Persist_Values()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseNodaTime());

        var duration = Duration.FromMilliseconds(1000);
        var instant = Instant.FromUnixTimeMilliseconds(1000);
        var datetime = new LocalDateTime(2000, 1, 1, 1, 1);
        var offset = Offset.FromMilliseconds(-1000);
        var zone = DateTimeZone.ForOffset(offset);

        var testObject = new NodaTypesModel
        {
            Guid = Guid.NewGuid(),
            Duration = duration,
            Instant = instant,
            LocalDateTime = datetime,
            LocalDate = datetime.Date,
            LocalTime = datetime.TimeOfDay,
            OffsetDateTime = new OffsetDateTime(datetime, offset),
            Offset = offset,
            ZonedDateTime = new ZonedDateTime(instant, zone),
            NullableDuration = duration,
            NullableInstant = instant,
            NullableLocalDateTime = datetime,
            NullableLocalDate = datetime.Date,
            NullableLocalTime = datetime.TimeOfDay,
            NullableOffsetDateTime = new OffsetDateTime(datetime, offset),
            NullableOffset = offset,
            NullableZonedDateTime = new ZonedDateTime(instant, zone)
        };

        dbContext.Add(testObject);

        await dbContext.SaveChangesAsync();

        var persistedTestObject = await dbContext.NodaTypesModels.SingleAsync();

        Assert.Equal(testObject.Guid, persistedTestObject.Guid);
        Assert.Equal(testObject.Duration, persistedTestObject.Duration);
        Assert.Equal(testObject.Instant, persistedTestObject.Instant);
        Assert.Equal(testObject.LocalDateTime, persistedTestObject.LocalDateTime);
        Assert.Equal(testObject.LocalDate, persistedTestObject.LocalDate);
        Assert.Equal(testObject.LocalTime, persistedTestObject.LocalTime);
        Assert.Equal(testObject.OffsetDateTime, persistedTestObject.OffsetDateTime);
        Assert.Equal(testObject.Offset, persistedTestObject.Offset);
        Assert.Equal(testObject.ZonedDateTime, persistedTestObject.ZonedDateTime);
        Assert.Equal(testObject.NullableDuration, persistedTestObject.NullableDuration);
        Assert.Equal(testObject.NullableInstant, persistedTestObject.NullableInstant);
        Assert.Equal(testObject.NullableLocalDateTime, persistedTestObject.NullableLocalDateTime);
        Assert.Equal(testObject.NullableLocalDate, persistedTestObject.NullableLocalDate);
        Assert.Equal(testObject.NullableLocalTime, persistedTestObject.NullableLocalTime);
        Assert.Equal(testObject.NullableOffsetDateTime, persistedTestObject.NullableOffsetDateTime);
        Assert.Equal(testObject.NullableOffset, persistedTestObject.NullableOffset);
        Assert.Equal(testObject.NullableZonedDateTime, persistedTestObject.NullableZonedDateTime);
    }

    [Fact]
    public async Task Should_Persist_Default_Values()
    {
        using var dbContext = SqliteTestDbContextActivator.Create<TestDbContext>(null, x => x.UseNodaTime());

        var testObject = new NodaTypesModel
        {
            Guid = Guid.NewGuid(),
            Duration = default,
            Instant = default,
            LocalDateTime = default,
            LocalDate = default,
            LocalTime = default,
            OffsetDateTime = default,
            Offset = default,
            ZonedDateTime = default,
            NullableDuration = default,
            NullableInstant = default,
            NullableLocalDateTime = default,
            NullableLocalDate = default,
            NullableLocalTime = default,
            NullableOffsetDateTime = default,
            NullableOffset = default,
            NullableZonedDateTime = default
        };

        dbContext.Add(testObject);

        await dbContext.SaveChangesAsync();

        var persistedTestObject = await dbContext.NodaTypesModels.SingleAsync();

        Assert.Equal(testObject.Guid, persistedTestObject.Guid);
        Assert.Equal(testObject.Duration, persistedTestObject.Duration);
        Assert.Equal(testObject.Instant, persistedTestObject.Instant);
        Assert.Equal(testObject.LocalDateTime, persistedTestObject.LocalDateTime);
        Assert.Equal(testObject.LocalDate, persistedTestObject.LocalDate);
        Assert.Equal(testObject.LocalTime, persistedTestObject.LocalTime);
        Assert.Equal(testObject.OffsetDateTime, persistedTestObject.OffsetDateTime);
        Assert.Equal(testObject.Offset, persistedTestObject.Offset);
        Assert.Equal(testObject.ZonedDateTime, persistedTestObject.ZonedDateTime);
        Assert.Equal(testObject.NullableDuration, persistedTestObject.NullableDuration);
        Assert.Equal(testObject.NullableInstant, persistedTestObject.NullableInstant);
        Assert.Equal(testObject.NullableLocalDateTime, persistedTestObject.NullableLocalDateTime);
        Assert.Equal(testObject.NullableLocalDate, persistedTestObject.NullableLocalDate);
        Assert.Equal(testObject.NullableLocalTime, persistedTestObject.NullableLocalTime);
        Assert.Equal(testObject.NullableOffsetDateTime, persistedTestObject.NullableOffsetDateTime);
        Assert.Equal(testObject.NullableOffset, persistedTestObject.NullableOffset);
        Assert.Equal(testObject.NullableZonedDateTime, persistedTestObject.NullableZonedDateTime);
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