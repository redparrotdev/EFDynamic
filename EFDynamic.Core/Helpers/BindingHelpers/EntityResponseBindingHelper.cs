using EFDynamic.Core.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace EFDynamic.Core.Helpers;
public static partial class BindingHelper
{
    public static Expression BuildEntityResponseInstanceInitExpression(
        Expression propertiesInitExpression
        , Expression? relatedEntitiesInitExpression
        , Expression? additionalPropertiesInitExpression = null)
    {
        var ctor = EntityResponseInfo.Ctor;
        var propertiesPropertyInfo = EntityResponseInfo.PropertiesPropertyInfo;

        IEnumerable<MemberBinding> bindings = [
            Expression.Bind(
                propertiesPropertyInfo
                , propertiesInitExpression)
        ];

        if (relatedEntitiesInitExpression is not null)
        {
            var relatedEntitiesPropertyInfo = EntityResponseInfo.RelatedEntitiesPropertyInfo;

            bindings = bindings.Append(
                Expression.Bind(
                    relatedEntitiesPropertyInfo
                    , relatedEntitiesInitExpression));
        }

        if (additionalPropertiesInitExpression is not null)
        {
            var additionalPropertiesPropertyInfo = EntityResponseInfo.AdditionalPropertiesPropertyInfo;
            bindings = bindings.Append(
                Expression.Bind(
                    additionalPropertiesPropertyInfo
                    , additionalPropertiesInitExpression));
        }

        var initExpression = Expression.MemberInit(
            Expression.New(ctor)
            , bindings);

        return initExpression;
    }

    public static class EntityResponseInfo
    {
        public static readonly Type Type = typeof(EntityResponse);

        public static readonly ConstructorInfo Ctor
            = typeof(EntityResponse).GetConstructor(Type.EmptyTypes)!;

        public static readonly PropertyInfo PropertiesPropertyInfo
            = typeof(EntityResponse).GetProperty(nameof(EntityResponse.Properties))!;

        public static readonly PropertyInfo RelatedEntitiesPropertyInfo
            = typeof(EntityResponse).GetProperty(nameof(EntityResponse.RelatedEntities))!;

        public static readonly PropertyInfo AdditionalPropertiesPropertyInfo
            = typeof(EntityResponse).GetProperty(nameof(EntityResponse.AdditionalProperties))!;
    }


}
