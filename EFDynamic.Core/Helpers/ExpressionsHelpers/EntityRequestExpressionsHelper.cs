using EFDynamic.Core.Extensions;
using EFDynamic.Core.Models;
using EFDynamic.Core.Utility;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EFDynamic.Core.Helpers;

public static partial class ExpressionsHelper
{
    public static LambdaExpression BuildSelectExpressionFromEntityRequest(
        DbContext context
        , EntityRequest request)
    {
        var parametersFactory = new ParametersFactory();

        var rootEntityType = context.GetEntityTypeByName(request.EntityName);
        var rootEntityParameter = parametersFactory.CreateParameter(rootEntityType);

        var rootEntityPropertiesExpression = BindingHelper.BuildEntityPropertiesArrayInitExpression(
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

        Expression? additionalPropertiesExpression = null;
        if (request.AdditionalProperties.Count > 0)
        {
            additionalPropertiesExpression = BuildAdditionalPropertiesExpression(
                rootEntityParameter
                , request.AdditionalProperties);
        }

        var rootEntityResponseExpression = BindingHelper.BuildEntityResponseInstanceInitExpression(
            rootEntityPropertiesExpression
            , rootRelatedEntitiesBinding
            , additionalPropertiesExpression);

        var lambda = Expression.Lambda(
            rootEntityResponseExpression
            , rootEntityParameter);

        return lambda;

        Expression BuildChildEntityBinding(RelatedEntityRequest req
            , Expression parentEntityParameter)
        {
            var parentEntityType = parentEntityParameter.Type;

            var (entityType, isCollection) = context.GetEntityNavigationType(
                parentEntityType
                , req.NavigationName);

            Expression childEntityParameter;
            if (isCollection)
            {
                childEntityParameter = parametersFactory.CreateParameter(entityType);
            }
            else
            {
                childEntityParameter = Expression.Property(
                    parentEntityParameter
                    , req.NavigationName);
            }

            var childEntityPropertiesExpression = BindingHelper.BuildEntityPropertiesArrayInitExpression(
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

            Expression? childAdditionalProperties = null;
            if (req.AdditionalProperties.Count > 0)
            {
                childAdditionalProperties = BuildAdditionalPropertiesExpression(
                    childEntityParameter
                    , req.AdditionalProperties);
            }

            var childEntityResponseExpression = BindingHelper.BuildEntityResponseInstanceInitExpression(
                childEntityPropertiesExpression
                , childRelatedEntitiesBinding
                , childAdditionalProperties);

            var childEntityExpression = isCollection
                ? BindingHelper.BuildRelatedEntityResponseCollectionExpression(
                    parentEntityParameter
                    , entityType
                    , req.NavigationName
                    , Expression.Lambda(
                        childEntityResponseExpression
                        , (ParameterExpression)childEntityParameter)
                    , req.WhereExpression
                    , req.Skip
                    , req.Take
                    , req.OrderBy
                    , req.Descending)
                : childEntityResponseExpression;

            var childEntityRelatedEntityResponseExpression = BindingHelper.BuildRelatedEntityResponseInstanceInitExpression(
                req.NavigationName
                , isCollection
                , childEntityExpression);

            return childEntityRelatedEntityResponseExpression;
        }

        static NewArrayExpression BuildAdditionalPropertiesExpression(
            Expression entityParameter
            , IEnumerable<Func<Expression, (string, Expression)>> factories)
        {
            var initializers = factories
                .Select(factory =>
                {
                    var (name, valueExpression) = factory(entityParameter);

                    var entityPropertyModelBinding = BindingHelper
                        .BuildEntityPropertyInstanceInitExpression(
                            name
                            , valueExpression);

                    return entityPropertyModelBinding;
                });

            var arrayInit = Expression.NewArrayInit(
                typeof(EntityPropertyModel)
                , initializers);

            return arrayInit;
        }
    }
}
