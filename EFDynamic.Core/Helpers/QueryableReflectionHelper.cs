using System.Reflection;

namespace EFDynamic.Core.Helpers;

public static class QueryableReflectionHelper
{
    private static readonly MethodInfo WhereMethodInfo
        = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m =>
                m.Name == nameof(Queryable.Where)
                && m.GetParameters().Length == 2);

    private static readonly MethodInfo SelectMethodInfo
        = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m =>
                m.Name == nameof(Queryable.Select)
                && m.GetParameters().Length == 2);

    public static MethodInfo GenericWhere(Type entityType)
    {
        return WhereMethodInfo.MakeGenericMethod(entityType);
    }

    public static MethodInfo GenericSelect(Type sourceType, Type resultType)
    {
        return SelectMethodInfo.MakeGenericMethod(sourceType, resultType);
    }
}
