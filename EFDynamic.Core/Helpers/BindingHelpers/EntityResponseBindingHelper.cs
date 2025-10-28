using EFDynamic.Core.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace EFDynamic.Core.Helpers;
public static partial class BindingHelper
{
    /// <summary>
    /// Builds an <see cref="Expression"/> that initializes an instance of the <see cref="EntityResponse"/> type with the
    /// specified property initialization expressions.
    /// </summary>
    /// <remarks>This method constructs a <see cref="MemberInitExpression"/> that initializes an
    /// <see cref="EntityResponse"/> object. The resulting expression can be used in LINQ expression trees to dynamically
    /// create instances of the type.</remarks>
    /// <param name="propertiesInitExpression">
    /// An <see cref="Expression"/> representing the initialization of the <see cref="EntityResponse.Properties"/>.
    /// This parameter is required and cannot be <c>null</c>.
    /// </param>
    /// <param name="relatedEntitiesInitExpression">
    /// An optional <see cref="Expression"/> representing the initialization of the <see cref="EntityResponse.RelatedEntities"/> property.
    /// </param>
    /// <param name="additionalPropertiesInitExpression">
    /// An optional <see cref="Expression"/> representing the initialization of the <see cref="EntityResponse.AdditionalProperties"/> property.
    /// </param>
    /// <returns>An <see cref="Expression"/> that represents the initialization of an <see cref="EntityResponse"/> instance with the
    /// specified property values.</returns>
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
