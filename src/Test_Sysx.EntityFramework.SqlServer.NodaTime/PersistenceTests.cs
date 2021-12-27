namespace Test_Sysx.EntityFramework.Sqlite.Identifiers;
using Assert = Assert;

public class PersistenceTests
{
    [Fact]
    public async Task Should_Persist_Values()
    {
        using var dbContext = SqlServerTestDbContextActivator.Create<TestDbContext>();

        var testObject = new NodaTypesModel
        {
            Guid = Guid.NewGuid(),
            Duration = Duration.FromMilliseconds(1000),
            Instant = Instant.FromUnixTimeMilliseconds(1000),
            LocalDateTime = LocalDateTime.FromDateTime(DateTime.UtcNow),
            LocalDate = LocalDate.FromDateTime(DateTime.UtcNow),
            LocalTime = LocalTime.FromMillisecondsSinceMidnight(1000),
            OffsetDateTime = OffsetDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow),
            Offset = Offset.FromMilliseconds(1000),
            ZonedDateTime = ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow),
            NullableDuration = Duration.FromMilliseconds(1000),
            NullableInstant = Instant.FromUnixTimeMilliseconds(1000),
            NullableLocalDateTime = LocalDateTime.FromDateTime(DateTime.UtcNow),
            NullableLocalDate = LocalDate.FromDateTime(DateTime.UtcNow),
            NullableLocalTime = LocalTime.FromMillisecondsSinceMidnight(1000),
            NullableOffsetDateTime = OffsetDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow),
            NullableOffset = Offset.FromMilliseconds(1000),
            NullableZonedDateTime = ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow)
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
        using var dbContext = SqlServerTestDbContextActivator.Create<TestDbContext>();

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