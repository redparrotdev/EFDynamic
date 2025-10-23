using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EFDynamic.Core.Helpers;

public static class DbContextHelper
{
    private static readonly MethodInfo DbContextSetMethodInfo
        = typeof(DbContext)
            .GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!;

    public static MethodInfo GenericSetMethod(Type entityType)
    {
        var genericSetMethod = DbContextSetMethodInfo
            .MakeGenericMethod(entityType)
            ?? throw new InvalidOperationException(
                $"Could not make generic method for DbContext.Set<> of type '{entityType.Name}'");

        return genericSetMethod;
    }
}
