using EFDynamic.Core.Extensions;
using EFDynamic.Core.Helpers;
using EFDynamic.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EFDynamic.Core;

public sealed class EntityDynamicService
{
    private int _nextParameterIndex = 0;

    public DbContext Context { get; }

    public EntityDynamicService(DbContext context)
    {
        Context = context;
    }

    public async Task<EntityResponse[]> Fetch(EntityRequest request)
    {
        var selectExpression = BuildSelectExpression(request);
        
        var queryable = Context.DynamicQuery(
            request.EntityName
            , request.WhereExpression
            , selectExpression);

        var typedQueryable = queryable.Provider.CreateQuery<EntityResponse>(queryable.Expression);

        var result = await typedQueryable
            .AsNoTracking()
            .ToArrayAsync();

        return result;
    }

    private LambdaExpression BuildSelectExpression(EntityRequest request)
    {
        var rootEntityParameterName = NextParameterName();
        var rootEntityType = Context.GetEntityTypeByName(request.EntityName);
        var rootEntityParameter = Expression.Parameter(
            rootEntityType
            , rootEntityParameterName);

        var rootEntityPropertiesExpression = BindingHelper.BuildEntityPropertiesBindings(
            rootEntityType
            , rootEntityParameter
            , request.Properties);

        var rootEntityChildsBindings = request
            .RelatedEntities
            .Select(re => BuildChildEntityBinding(re, rootEntityParameter));

        var rootRelatedEntitiesBinding = rootEntityChildsBindings.Any()
            ? Expression.NewArrayInit(
                typeof(RelatedEntityResponse)
                , rootEntityChildsBindings)
            : null;

        var rootEntityResponseExpression = BindingHelper.BuildEntityResponseBindings(
            rootEntityPropertiesExpression
            , rootRelatedEntitiesBinding);

        var lambda = Expression.Lambda(
            rootEntityResponseExpression
            , rootEntityParameter);

        return lambda;

        Expression BuildChildEntityBinding(RelatedEntityRequest req
            , Expression parentEntityParameter)
        {
            var parentEntityType = parentEntityParameter.Type;

            var (entityType, isCollection) = Context.GetEntityNavigationType(
                parentEntityType
                , req.NavigationName);

            Expression childEntityParameter;
            if (isCollection)
            {
                var childEntityParameterName = NextParameterName();
                childEntityParameter = Expression.Parameter(
                    entityType
                    , childEntityParameterName);
            }
            else
            {
                childEntityParameter = Expression.Property(
                    parentEntityParameter
                    , req.NavigationName);
            }

            var childEntityPropertiesExpression = BindingHelper.BuildEntityPropertiesBindings(
                entityType
                , childEntityParameter
                , req.Properties);

            var childEntityChildsBindings = req
                .RelatedEntities
                .Select(re => BuildChildEntityBinding(re, childEntityParameter));

            var childRelatedEntitiesBinding = childEntityChildsBindings.Any()
                ? Expression.NewArrayInit(
                    typeof(RelatedEntityResponse)
                    , childEntityChildsBindings)
                : null;

            var childEntityResponseExpression = BindingHelper.BuildEntityResponseBindings(
                childEntityPropertiesExpression
                , childRelatedEntitiesBinding);

            var childEntityExpression = isCollection
                ? BindingHelper.BuildRelatedEntityResponseCollectionBinding(
                    parentEntityParameter
                    , entityType
                    , req.NavigationName
                    , Expression.Lambda(
                        childEntityResponseExpression
                        , (ParameterExpression)childEntityParameter)
                    , req.WhereExpression)
                : childEntityResponseExpression;

            var childEntityRelatedEntityResponseExpression = BindingHelper.BuildRelatedEntityResponseBinding(
                req.NavigationName
                , isCollection
                , childEntityExpression);

            return childEntityRelatedEntityResponseExpression;
        }
    }

    private string NextParameterName()
    {
        var paramName = $"p_{_nextParameterIndex++}";
        return paramName;
    }
}

file static class LocalExtensions
{
    public static IQueryable DynamicQuery(this DbContext context
        , string entityName
        , LambdaExpression whereExpression
        , LambdaExpression selectExpression)
    {
        var entityType = context.GetEntityTypeByName(entityName);
        var entityQueryable = context.EntityQueryable(entityType);

        var whereMethod = QueryableReflectionHelper.GenericWhere(entityType);
        var selectMethod = QueryableReflectionHelper.GenericSelect(entityType, typeof(EntityResponse));

        var filtered = whereMethod.Invoke(null, [entityQueryable, whereExpression]);
        var projected = selectMethod.Invoke(null, [filtered!, selectExpression]);

        return (IQueryable)projected!;
    }
}
