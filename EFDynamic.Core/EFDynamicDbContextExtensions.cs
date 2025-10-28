using EFDynamic.Core.Extensions;
using EFDynamic.Core.Helpers;
using EFDynamic.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EFDynamic.Core;

public static class EFDynamicDbContextExtensions
{
    public static async Task<EntityResponse[]> FetchAsync(this DbContext context
        , EntityRequest request
        , bool asNoTracking = true
        , bool asSplitQuery = true)
    {
        var responseQueryable = context.BuildEntityResponseQueryable(
            request
            , asNoTracking
            , asSplitQuery);

        responseQueryable = responseQueryable.ApplyPagination(
            request.Skip
            , request.Take);

        var result = await responseQueryable.ToArrayAsync();

        return result;
    }

    public static async Task<(EntityResponse[], int count)> FetchWithCountAsync(this DbContext context
        , EntityRequest request
        , bool asNoTracking = true
        , bool asSplitQuery = true)
    {
        var responseQueryable = context.BuildEntityResponseQueryable(
            request
            , asNoTracking
            , asSplitQuery);

        var count = await responseQueryable.CountAsync();

        responseQueryable = responseQueryable.ApplyPagination(
            request.Skip
            , request.Take);

        var result = await responseQueryable.ToArrayAsync();

        return (result, count);
    }

    private static IQueryable<EntityResponse> BuildEntityResponseQueryable(this DbContext context
        , EntityRequest request
        , bool asNoTracking
        , bool asSplitQuery)
    {
        var rootEntityType = context.GetEntityTypeByName(request.EntityName);

        var selectExpression = ExpressionsHelper.BuildSelectExpressionFromEntityRequest(context, request);

        var dynamicQueryable = context.BuildDynamicQueryable(
            rootEntityType
            , selectExpression
            , request.WhereExpression
            , request.OrderBy
            , request.Descending);

        var typedQueryable = dynamicQueryable.Provider.CreateQuery<EntityResponse>(dynamicQueryable.Expression);

        typedQueryable = typedQueryable
            .IfThen(asNoTracking, q => q.AsNoTracking())
            .IfThen(asSplitQuery, q => q.AsSplitQuery());

        return typedQueryable;
    }

    private static IQueryable BuildDynamicQueryable(this DbContext context
        , Type entityType
        , LambdaExpression selectExpression
        , LambdaExpression? whereExpression = null
        , LambdaExpression? orderByExpression = null
        , bool descending = false)
    {
        var entityQueryable = (object)context.EntityQueryable(entityType);

        if (whereExpression is not null)
        {
            var whereMethod = QueryableReflectionHelper.GenericWhere(entityType);
            entityQueryable = whereMethod.Invoke(null, [entityQueryable, whereExpression]);
        }

        if (orderByExpression is not null)
        {
            var orderByMethod = QueryableReflectionHelper.GenericOrderBy(
                entityType
                , descending);

            entityQueryable = orderByMethod.Invoke(null, [entityQueryable!, orderByExpression]);
        }

        var selectMethod = QueryableReflectionHelper.GenericSelect(entityType, typeof(EntityResponse));
        var projected = selectMethod.Invoke(null, [entityQueryable!, selectExpression]);

        return (IQueryable)projected!;
    }

    private static IQueryable<EntityResponse> ApplyPagination(this IQueryable<EntityResponse> source
        , int? skip
        , int? take)
    {
        return source
            .IfThen(skip is not null, q => q.Skip(skip!.Value))
            .IfThen(take is not null, q => q.Take(take!.Value));
    }
}

file static class LocalExtensions
{

    public static IQueryable<T> IfThen<T>(
        this IQueryable<T> source
        , bool condition
        , Func<IQueryable<T>, IQueryable<T>> mapFunction)
    {
        if (condition)
        {
            return mapFunction.Invoke(source);
        }

        return source;
    }
}
