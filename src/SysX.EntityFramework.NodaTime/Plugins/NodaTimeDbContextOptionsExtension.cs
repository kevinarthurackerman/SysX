namespace SysX.EntityFramework.NodaTime.Plugins;

/// <summary>
/// ContainerTypesDbContextOptionsExtension that adds handling of NodaTime types to EntityFramework
/// </summary>
public sealed class NodaTimeDbContextOptionsExtension : BaseContainerTypesDbContextOptionsExtension
{
	public override void RegisterServices(IServiceCollection services, IDatabaseProvider databaseProvider)
	{
		EnsureArg.IsNotNull(services, nameof(services));
		EnsureArg.IsNotNull(databaseProvider, nameof(databaseProvider));

		services.AddSingleton<RelationalTypeMapping>(services =>
		{
			var initializeRelationalTypeMapper = () =>
			{
				var providerTypeMapping = services
					.GetRequiredService<IRelationalTypeMappingSource>()
					.FindMapping(typeof(short));

				return (RelationalTypeMapping)providerTypeMapping
					.Clone(new RelationalTypeMappingInfo(typeof(AnnualDate)))
					.Clone(new ValueConverter<AnnualDate, short>(x => (short)((x.Month * 100) + x.Day), x => new AnnualDate(x / 100, x % 100)));
			};

			return new LazyInitializedRelationalTypeMapping(typeof(AnnualDate), initializeRelationalTypeMapper);
		});

		services.AddSingleton<RelationalTypeMapping>(services =>
		{
			var initializeRelationalTypeMapper = () =>
			{
				var providerTypeMapping = services
					.GetRequiredService<IRelationalTypeMappingSource>()
					.FindMapping(typeof(long));

				return (RelationalTypeMapping)providerTypeMapping
					.Clone(new RelationalTypeMappingInfo(typeof(Duration)))
					.Clone(new ValueConverter<Duration, long>(x => x.BclCompatibleTicks, x => Duration.FromTicks(x)));
			};

			return new LazyInitializedRelationalTypeMapping(typeof(Duration), initializeRelationalTypeMapper);
		});

		services.AddSingleton<RelationalTypeMapping>(services =>
		{
			var initializeRelationalTypeMapper = () =>
			{
				var providerTypeMapping = services
					.GetRequiredService<IRelationalTypeMappingSource>()
					.FindMapping(typeof(DateTime));

				return (RelationalTypeMapping)providerTypeMapping
					.Clone(new RelationalTypeMappingInfo(typeof(Instant)))
					.Clone(new ValueConverter<Instant, DateTime>(x => x.ToDateTimeUtc(), x => Instant.FromDateTimeUtc(x)));
			};

			return new LazyInitializedRelationalTypeMapping(typeof(Instant), initializeRelationalTypeMapper);
		});

		services.AddSingleton<RelationalTypeMapping>(services =>
		{
			var initializeRelationalTypeMapper = () =>
			{
				var providerTypeMapping = services
					.GetRequiredService<IRelationalTypeMappingSource>()
					.FindMapping(typeof(string));

				var storeType = databaseProvider.IsSqlServer()
					? "varchar(30)"
					: providerTypeMapping.StoreType;

				return (RelationalTypeMapping)providerTypeMapping
					.Clone(new RelationalTypeMappingInfo(typeof(Interval)))
					.Clone(new ValueConverter<Interval, string>(
						x => x.ToString(), x => ParseInterval(x)));
			};

			return new LazyInitializedRelationalTypeMapping(typeof(Interval), initializeRelationalTypeMapper);
		});

		services.AddSingleton<RelationalTypeMapping>(services =>
		{
			var initializeRelationalTypeMapper = () =>
			{
				var providerTypeMapping = services
					.GetRequiredService<IRelationalTypeMappingSource>()
					.FindMapping(typeof(DateTime));

				return (RelationalTypeMapping)providerTypeMapping
					.Clone(new RelationalTypeMappingInfo(typeof(LocalDateTime)))
					.Clone(new ValueConverter<LocalDateTime, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDateTime.FromDateTime(x)));
			};

			return new LazyInitializedRelationalTypeMapping(typeof(LocalDateTime), initializeRelationalTypeMapper);
		});

		services.AddSingleton<RelationalTypeMapping>(services =>
		{
			var initializeRelationalTypeMapper = () =>
			{
				if (databaseProvider.IsSqlite())
				{
					var providerTypeMapping = services
						.GetRequiredService<IRelationalTypeMappingSource>()
						.FindMapping(typeof(string));

					return (RelationalTypeMapping)providerTypeMapping
						.Clone(new RelationalTypeMappingInfo(typeof(string)))
						.Clone(new ValueConverter<LocalDate, string>(
							x => x.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
							x => LocalDatePattern.Iso.Parse(x).GetValueOrThrow()));
				}
				else
				{
					var providerTypeMapping = services
						.GetRequiredService<IRelationalTypeMappingSource>()
						.FindMapping(typeof(DateTime));

					var storeType = databaseProvider.IsSqlServer()
						? "date"
						: providerTypeMapping.StoreType;

					return (RelationalTypeMapping)providerTypeMapping
						.Clone(new RelationalTypeMappingInfo(typeof(LocalDate), storeTypeName: storeType))
						.Clone(new ValueConverter<LocalDate, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDate.FromDateTime(x)));
				}
			};

			return new LazyInitializedRelationalTypeMapping(typeof(LocalDate), initializeRelationalTypeMapper);
		});

		services.AddSingleton<RelationalTypeMapping>(services =>
		{
			var initializeRelationalTypeMapper = () =>
			{
				var providerTypeMapping = services
					.GetRequiredService<IRelationalTypeMappingSource>()
					.FindMapping(typeof(TimeSpan));

				return (RelationalTypeMapping)providerTypeMapping
					.Clone(new RelationalTypeMappingInfo(typeof(LocalTime)))
					.Clone(new ValueConverter<LocalTime, TimeSpan>(x => TimeSpan.FromTicks(x.TickOfDay), x => LocalTime.FromTicksSinceMidnight(x.Ticks)));
			};

			return new LazyInitializedRelationalTypeMapping(typeof(LocalTime), initializeRelationalTypeMapper);
		});

		services.AddSingleton<RelationalTypeMapping>(services =>
		{
			var initializeRelationalTypeMapper = () =>
			{
				var providerTypeMapping = services
					.GetRequiredService<IRelationalTypeMappingSource>()
					.FindMapping(typeof(DateTimeOffset));

				return (RelationalTypeMapping)providerTypeMapping
					.Clone(new RelationalTypeMappingInfo(typeof(OffsetDateTime)))
					.Clone(new ValueConverter<OffsetDateTime, DateTimeOffset>(x => x.ToDateTimeOffset(), x => OffsetDateTime.FromDateTimeOffset(x)));
			};

			return new LazyInitializedRelationalTypeMapping(typeof(OffsetDateTime), initializeRelationalTypeMapper);
		});

		services.AddSingleton<RelationalTypeMapping>(services =>
		{
			var initializeRelationalTypeMapper = () =>
			{
				var providerTypeMapping = services
					.GetRequiredService<IRelationalTypeMappingSource>()
					.FindMapping(typeof(int));

				return (RelationalTypeMapping)providerTypeMapping
					.Clone(new RelationalTypeMappingInfo(typeof(Offset)))
					.Clone(new ValueConverter<Offset, int>(x => x.Seconds, x => Offset.FromSeconds(x)));
			};

			return new LazyInitializedRelationalTypeMapping(typeof(Offset), initializeRelationalTypeMapper);
		});

		services.AddSingleton<RelationalTypeMapping>(services =>
		{
			var initializeRelationalTypeMapper = () =>
			{
				var providerTypeMapping = services
					.GetRequiredService<IRelationalTypeMappingSource>()
					.FindMapping(typeof(string));

				var storeType = databaseProvider.IsSqlServer()
					? "varchar(max)"
					: providerTypeMapping.StoreType;

				return (RelationalTypeMapping)providerTypeMapping
					.Clone(new RelationalTypeMappingInfo(typeof(Period)))
					.Clone(new ValueConverter<Period, string>(
						x => x.ToString(), x => PeriodPattern.Roundtrip.Parse(x).GetValueOrThrow()));
			};

			return new LazyInitializedRelationalTypeMapping(typeof(Period), initializeRelationalTypeMapper);
		});

		services.AddSingleton<RelationalTypeMapping>(services =>
		{
			var initializeRelationalTypeMapper = () =>
			{
				var providerTypeMapping = services
					.GetRequiredService<IRelationalTypeMappingSource>()
					.FindMapping(typeof(DateTimeOffset));

				return (RelationalTypeMapping)providerTypeMapping
					.Clone(new RelationalTypeMappingInfo(typeof(ZonedDateTime)))
					.Clone(new ValueConverter<ZonedDateTime, DateTimeOffset>(x => x.ToDateTimeOffset(), x => ZonedDateTime.FromDateTimeOffset(x)));
			};

			return new LazyInitializedRelationalTypeMapping(typeof(ZonedDateTime), initializeRelationalTypeMapper);
		});
	}

	static private Interval ParseInterval(string value)
	{
		var fragments = value.Split('/');

		Ensure.Collection.SizeIs(fragments, 2, nameof(value), x => x.WithMessage($"Invalid format for {nameof(Interval)} type."));

		var start = InstantPattern.General.Parse(fragments[0]).GetValueOrThrow();
		var end = InstantPattern.General.Parse(fragments[1]).GetValueOrThrow();

		return new Interval(start, end);
	}
}