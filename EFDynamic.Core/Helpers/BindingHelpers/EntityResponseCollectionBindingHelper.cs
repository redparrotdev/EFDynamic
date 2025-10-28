using EFDynamic.Core.Models;
using System.Linq.Expressions;

namespace EFDynamic.Core.Helpers;
public static partial class BindingHelper
{
    /// <summary>
    /// Builds an expression that generates a collection of related entity responses based on the specified parameters.
    /// </summary>
    /// <remarks>This method constructs a LINQ expression that applies filtering, ordering, skipping, and
    /// taking operations to a collection of related entities, and then projects the results into an array of
    /// <see cref="RelatedEntityResponse"/> objects. The resulting expression can be used in query providers or other dynamic query
    /// scenarios.</remarks>
    /// <param name="parentEntityParameter">An expression representing the parent entity from which the related entities are accessed.</param>
    /// <param name="targetEntityType">The type of the target related entity.</param>
    /// <param name="navigatioName">The name of the navigation property on the parent entity that points to the related entities.</param>
    /// <param name="selector">A lambda expression used to project each related entity into an <see cref="RelatedEntityResponse"/>.</param>
    /// <param name="filter">An optional lambda expression used to filter the related entities. If <c>null</c>, no filtering is applied.</param>
    /// <param name="skip">An optional number of items to skip in the related entity collection. If <c>null</c>, no items are skipped.</param>
    /// <param name="take">An optional number of items to take from the related entity collection. If <c>null</c>, all items are taken.</param>
    /// <param name="orderBy">An optional lambda expression used to order the related entities. If <c>null</c>, no ordering is applied.</param>
    /// <param name="descending">A value indicating whether the ordering specified by <paramref name="orderBy"/> should be in descending order.
    /// Ignored if <paramref name="orderBy"/> is <c>null</c>.</param>
    /// <returns>An <see cref="Expression"/> that, when executed, produces an array of <see cref="RelatedEntityResponse"/> objects representing
    /// the related entities.</returns>
    public static Expression BuildRelatedEntityResponseCollectionExpression(
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
