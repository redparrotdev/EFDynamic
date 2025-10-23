using EFDynamic.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EFDynamic.Core.Extensions;

public static class DbContextExtensions
{
    public static Type GetEntityTypeByName(this DbContext context,
        string entityName)
    {
        return context
            .Model
            .GetEntityTypes()
            .FirstOrDefault(t => t.DisplayName() == entityName)?
            .ClrType
            ?? throw new InvalidOperationException(
                $"Entity type '{entityName}' not found in the current context.");
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
}
