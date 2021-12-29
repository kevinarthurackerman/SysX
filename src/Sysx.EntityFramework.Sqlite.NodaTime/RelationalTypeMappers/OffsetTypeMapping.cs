namespace Sysx.EntityFramework.Sqlite.RelationalTypeMappings;

public class OffsetTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<Offset, string> converter = new(
            x => TimeSpan.FromTicks(x.Ticks).ToString(@"hh\:mm\:ss\.fffffff"),
            x => Offset.FromTicks(TimeSpan.ParseExact(x, @"hh\:mm\:ss\.fffffff", CultureInfo.InvariantCulture).Ticks)
        );

    public override Type ClrType => typeof(Offset);
    public override ValueConverter Converter => converter;
    public override string StoreType => "TEXT";
    public override DbType? DbType => System.Data.DbType.DateTimeOffset;
    public override int? Size => 14;
    public override bool IsFixedLength => true;

    public OffsetTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new OffsetTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}