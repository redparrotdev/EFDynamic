using System.Reflection;

namespace EFDynamic.Core.Helpers;

public static class EnumerableReflectionHelper
{
    public static readonly MethodInfo SelectMethodInfo
        = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(Enumerable.Select)
                && m.GetParameters().Length == 2);

    public static readonly MethodInfo WhereMethodInfo
        = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(Enumerable.Where)
                && m.GetParameters().Length == 2);

    public static readonly MethodInfo ToArrayMethodInfo
        = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(Enumerable.ToArray)
                && m.GetParameters().Length == 1);

    public static readonly MethodInfo SkipMethodInfo
        = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(Enumerable.Skip)
                && m.GetParameters().Length == 2);

    public static readonly MethodInfo TakeMethodInfo
        = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(Enumerable.Take)
                && m.GetParameters().Length == 2);

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
}
