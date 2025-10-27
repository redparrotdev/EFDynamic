using System.Reflection;

namespace EFDynamic.Core.Helpers;

public static class QueryableReflectionHelper
{
    public static readonly MethodInfo WhereMethodInfo
        = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m =>
                m.Name == nameof(Queryable.Where)
                && m.GetParameters().Length == 2);

    public static readonly MethodInfo OrderByMethodInfo 
        = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m =>
                m.Name == nameof(Queryable.OrderBy)
                && m.GetParameters().Length == 2);

    public static readonly MethodInfo OrderByDescendingMethodInfo
        = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m =>
                m.Name == nameof(Queryable.OrderByDescending)
                && m.GetParameters().Length == 2);

    public static readonly MethodInfo SelectMethodInfo
        = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m =>
                m.Name == nameof(Queryable.Select)
                && m.GetParameters().Length == 2);

    private static readonly Type ObjectType = typeof(object);

    public static MethodInfo GenericWhere(Type entityType)
    {
        return WhereMethodInfo.MakeGenericMethod(entityType);
    }

    public static MethodInfo GenericOrderBy(Type entityType, bool descending)
    {
        return descending
            ? OrderByDescendingMethodInfo.MakeGenericMethod(entityType, ObjectType)
            : OrderByMethodInfo.MakeGenericMethod(entityType, ObjectType);
    }

    public static MethodInfo GenericSelect(Type sourceType, Type resultType)
    {
        return SelectMethodInfo.MakeGenericMethod(sourceType, resultType);
    }
}
