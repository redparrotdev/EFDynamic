using EFDynamic.Core.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace EFDynamic.Core.Helpers;
public static partial class BindingHelper
{
    /// <summary>
    /// Constructs an expression that initializes an <see cref="EntityResponse"/> object with specified property and
    /// related entity bindings.
    /// </summary>
    /// <param name="propertiesBinding">The expression representing the binding for the properties of the entity.</param>
    /// <param name="relatedEntitiesBinding">The expression representing the binding for the related entities of the entity. Can be <see langword="null"/> if
    /// no related entities are to be bound.</param>
    /// <returns>An <see cref="Expression"/> that initializes an <see cref="EntityResponse"/> object with the specified
    /// bindings.</returns>
    public static Expression BuildEntityResponseBindings(
        Expression propertiesBinding
        , Expression? relatedEntitiesBinding
        , Expression? additionalProperties = null)
    {
        var ctor = EntityResponseInfo.Ctor;
        var propertiesPropertyInfo = EntityResponseInfo.PropertiesPropertyInfo;

        IEnumerable<MemberBinding> bindings = [
            Expression.Bind(
                propertiesPropertyInfo
                , propertiesBinding)
        ];

        if (relatedEntitiesBinding is not null)
        {
            var relatedEntitiesPropertyInfo = EntityResponseInfo.RelatedEntitiesPropertyInfo;

            bindings = bindings.Append(
                Expression.Bind(
                    relatedEntitiesPropertyInfo
                    , relatedEntitiesBinding));
        }

        if (additionalProperties is not null)
        {
            var additionalPropertiesPropertyInfo = EntityResponseInfo.AdditionalPropertiesPropertyInfo;
            bindings = bindings.Append(
                Expression.Bind(
                    additionalPropertiesPropertyInfo
                    , additionalProperties));
        }

        var initExpression = Expression.MemberInit(
            Expression.New(ctor)
            , bindings);

        return initExpression;
    }

    private static class EntityResponseInfo
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
