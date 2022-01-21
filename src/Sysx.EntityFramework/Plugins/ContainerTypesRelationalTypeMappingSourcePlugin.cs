namespace Sysx.EntityFramework.Plugins;

/// <summary>
/// The IRelationalTypeMappingSourcePlugin used by ContainerTypesDbContextOptionsExtensions to handle RelationalTypeMappings
/// </summary>
public class ContainerTypesRelationalTypeMappingSourcePlugin : IRelationalTypeMappingSourcePlugin
{
    private readonly ImmutableDictionary<Type, RelationalTypeMapping> relationalTypeMappingLookupByType;

    public ContainerTypesRelationalTypeMappingSourcePlugin(IEnumerable<RelationalTypeMapping> relationalTypeMappings)
    {
        EnsureArg.IsNotNull(relationalTypeMappings, nameof(relationalTypeMappings));

        var baseRelationalMappingForTypes = relationalTypeMappings
            .OfType<BaseRelationalMappingForType>()
            .ToDictionary(x => x.ForType, x => (RelationalTypeMapping)x);

        var otherRelationalTypeMappings = relationalTypeMappings
            .Where(x => x is not BaseRelationalMappingForType)
            .ToDictionary(x => x.ClrType);

        relationalTypeMappingLookupByType = baseRelationalMappingForTypes
            .Concat(otherRelationalTypeMappings)
            .ToImmutableDictionary();
    }

    public RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
    {
        relationalTypeMappingLookupByType.TryGetValue(mappingInfo.ClrType, out var mapping);
            
        return mapping;
    }
}