using EFDynamic.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EFDynamic.Core.Extensions;

public static class DbContextExtensions
{
    private static readonly Dictionary<string, Type> _entityTypeByNameCache = [];
    private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _entityTypePropertiesMappingCache = [];

    public static Type GetEntityTypeByName(this DbContext context,
        string entityName)
    {
        if (_entityTypeByNameCache.TryGetValue(entityName, out var entityType))
        {
            return entityType;
        }

        entityType = context
            .Model
            .GetEntityTypes()
            .FirstOrDefault(t => t.DisplayName() == entityName)?
            .ClrType
            ?? throw new InvalidOperationException(
                $"Entity type '{entityName}' not found in the current context.");

        _entityTypeByNameCache[entityName] = entityType;

        return entityType;
    }

    public static IQueryable EntityQueryable(this DbContext context
        , Type entityType)
    {
        var genericSetMethod = DbContextHelper.GenericSetMethod(entityType);

        var dbSet = genericSetMethod
            .Invoke(context, null)
            ?? throw new InvalidOperationException(
                $"Could not invoke DbContext.Set<> for type '{entityType.Name}'");

        return (IQueryable)dbSet;
    }

    public static (Type, bool) GetEntityNavigationType(this DbContext context
        , Type entityType
        , string navigationName)
    {
        var navigation = context
            .Model
            .FindEntityType(entityType)
            ?.FindNavigation(navigationName)
            ?? throw new InvalidOperationException(
                $"Navigation '{navigationName}' not found on entity type '{entityType.Name}'.");

        var navigationIsCollection = navigation.IsCollection;

        return (navigation.TargetEntityType.ClrType, navigationIsCollection);
    }

    public static Dictionary<string, PropertyInfo> GetEntityTypePropertiesMapping(this DbContext context
        , Type entityType)
    {
        if (_entityTypePropertiesMappingCache.TryGetValue(entityType, out var propertyInfoLookup))
        {
            return propertyInfoLookup;
        }

        var entityDbType = context
            .Model
            .FindEntityType(entityType)
            ?? throw new InvalidOperationException(
                $"Entity type '{entityType.Name}' not found in the current context.");

        var mapping = entityDbType
            .GetProperties()
            .Where(p => !p.IsShadowProperty())
            .ToDictionary(
                p => p.Name,
                p => p.PropertyInfo!
            );

        _entityTypePropertiesMappingCache[entityType] = mapping;

        return mapping;
    }
}
