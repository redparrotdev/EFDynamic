using System.Linq.Expressions;

namespace EFDynamic.Core.Helpers;

public static class ExpressionsHelper
{
    public static LambdaExpression WherePropertyEquals(Type entityType
        , string propertyName
        , object? propertyValue)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, propertyName);
        var constant = Expression.Constant(propertyValue);
        var equality = Expression.Equal(property, constant);
        var lambda = Expression.Lambda(equality, parameter);
        return lambda;
    }

    public static LambdaExpression WherePropertyGreaterThan(Type entityType
        , string propertyName
        , object? propertyValue)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, propertyName);
        var constant = Expression.Constant(propertyValue);
        var greaterThan = Expression.GreaterThan(property, constant);
        var lambda = Expression.Lambda(greaterThan, parameter);
        return lambda;
    }

    public static LambdaExpression OrderBy(Type entityType
        , string propertyName)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, propertyName);
        var convertedProperty = Expression.Convert(property, typeof(object));
        var lambda = Expression.Lambda(convertedProperty, parameter);
        return lambda;
    }
}
