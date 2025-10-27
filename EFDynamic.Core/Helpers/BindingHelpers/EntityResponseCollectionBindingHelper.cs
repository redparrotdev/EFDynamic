using EFDynamic.Core.Models;
using System.Linq.Expressions;

namespace EFDynamic.Core.Helpers;
public static partial class BindingHelper
{
    /// <summary>
    /// Builds an expression that projects a collection of related entities into an array of <see
    /// cref="EntityResponse"/> objects.
    /// </summary>
    /// <param name="parentEntityParameter">The parameter expression representing the parent entity from which the navigation property is accessed.</param>
    /// <param name="targetEntityType">The type of the target entity in the navigation property collection.</param>
    /// <param name="navigatioName">The name of the navigation property on the parent entity that represents the collection of related entities.</param>
    /// <param name="selector">A lambda expression used to transform each entity in the collection to an <see cref="EntityResponse"/>.</param>
    /// <returns>An expression that, when executed, returns an array of <see cref="EntityResponse"/> objects representing the
    /// related entities.</returns>
    public static Expression BuildRelatedEntityResponseCollectionBinding(
        Expression parentEntityParameter
        , Type targetEntityType
        , string navigatioName
        , LambdaExpression selector
        , LambdaExpression? filter = null
        , int? skip = null
        , int? take = null
        , LambdaExpression? orderBy = null
        , bool descending = false)
    {
        Expression navigationPropertyAccess = Expression.Property(
            parentEntityParameter
            , navigatioName);

        if (filter is not null)
        {
            var genericWhere = EnumerableReflectionHelper
                .GenericWhere(
                    targetEntityType);

            var whereCall = Expression.Call(
                genericWhere
                , navigationPropertyAccess
                , filter);

            navigationPropertyAccess = whereCall;
        }

        if (orderBy is not null)
        {
            var genericOrderBy = EnumerableReflectionHelper
                .GenericOrderBy(
                    targetEntityType
                    , descending);

            var orderByCall = Expression.Call(
                genericOrderBy
                , navigationPropertyAccess
                , orderBy);

            navigationPropertyAccess = orderByCall;
        }

        if (skip is int skipInt)
        {
            var genericSkip = EnumerableReflectionHelper
                .GenericSkip(
                    targetEntityType);

            var skipCall = Expression.Call(
                genericSkip
                , navigationPropertyAccess
                , Expression.Constant(skipInt));

            navigationPropertyAccess = skipCall;
        }

        if (take is int takeInt)
        {
            var genericTake = EnumerableReflectionHelper
                .GenericTake(
                    targetEntityType);

            var takeCall = Expression.Call(
                genericTake
                , navigationPropertyAccess
                , Expression.Constant(takeInt));

            navigationPropertyAccess = takeCall;
        }

        var genericSelect = EnumerableReflectionHelper
            .GenericSelect(
                targetEntityType
                , typeof(EntityResponse));

        var selectCall = Expression.Call(
            genericSelect
            , navigationPropertyAccess
            , selector);

        var toArrayMethod = EnumerableReflectionHelper
            .GenericToArray(
                typeof(EntityResponse));

        var toArrayCall = Expression.Call(
            toArrayMethod
            , selectCall);

        return toArrayCall;
    }
}
