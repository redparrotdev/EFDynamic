using System.Reflection;

namespace EFDynamic.Core.Helpers;

public static class EnumerableReflectionHelper
{
    public static readonly MethodInfo SelectMethodInfo
        = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(Enumerable.Select)
                && m.GetParameters().Length == 2);

    public static readonly MethodInfo WhereMethodInfo
        = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(Enumerable.Where)
                && m.GetParameters().Length == 2);

    public static readonly MethodInfo ToArrayMethodInfo
        = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(Enumerable.ToArray)
                && m.GetParameters().Length == 1);

    public static readonly MethodInfo SkipMethodInfo
        = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(Enumerable.Skip)
                && m.GetParameters().Length == 2);

    public static readonly MethodInfo TakeMethodInfo
        = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(Enumerable.Take)
                && m.GetParameters().Length == 2);

    public static readonly MethodInfo OrderByMethodInfo
        = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(Enumerable.OrderBy)
                && m.GetParameters().Length == 2);

    public static readonly MethodInfo OrderByDescendingMethodInfo
        = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(Enumerable.OrderByDescending)
                && m.GetParameters().Length == 2);

    private static readonly Type ObjectType = typeof(object);

    public static MethodInfo GenericSelect(Type tIn, Type tOut)
    {
        return SelectMethodInfo.MakeGenericMethod(tIn, tOut);
    }

    public static MethodInfo GenericWhere(Type tElement)
    {
        return WhereMethodInfo.MakeGenericMethod(tElement);
    }

    public static MethodInfo GenericToArray(Type tElement)
    {
        return ToArrayMethodInfo.MakeGenericMethod(tElement);
    }

    public static MethodInfo GenericSkip(Type tElement)
    {
        return SkipMethodInfo.MakeGenericMethod(tElement);
    }

    public static MethodInfo GenericTake(Type tElement)
    {
        return TakeMethodInfo.MakeGenericMethod(tElement);
    }

    public static MethodInfo GenericOrderBy(Type tElement, bool descending)
    {
        return descending
            ? OrderByDescendingMethodInfo.MakeGenericMethod(tElement, ObjectType)
            : OrderByMethodInfo.MakeGenericMethod(tElement, ObjectType);
    }
}
