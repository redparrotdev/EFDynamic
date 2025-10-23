using EFDynamic.Core.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace EFDynamic.Core.Helpers;
public static partial class BindingHelper
{
    /// <summary>
    /// Builds an expression that initializes a <see cref="RelatedEntityResponse"/> object with the specified navigation name,
    /// collection status, and related entity expression.
    /// </summary>
    /// <param name="navigationName">The name of the navigation property associated with the related entity.</param>
    /// <param name="isCollection">A value indicating whether the related entity is a collection. <see langword="true"/> if it is a collection;
    /// otherwise, <see langword="false"/>.</param>
    /// <param name="relatedEntityExpression">An expression representing the related entity or entities to be bound to the response.</param>
    /// <returns>An <see cref="Expression"/> that represents the initialization of a <see cref="RelatedEntityResponse"/> object with the
    /// specified parameters.</returns>
    public static Expression BuildRelatedEntityResponseBinding(
        string navigationName
        , bool isCollection
        , Expression relatedEntityExpression)
    {
        var ctor = RelatedEntityResponseInfo.Ctor;
        var navigationNamePropertyInfo = RelatedEntityResponseInfo.NavigationNamePropertyInfo;
        var isCollectionPropertyInfo = RelatedEntityResponseInfo.IsCollectionPropertyInfo;
        var entitiesPropertyInfo = RelatedEntityResponseInfo.EntitiesPropertyInfo;
        var entityPropertyInfo = RelatedEntityResponseInfo.EntityPropertyInfo;

        var targetPropertyInfo = isCollection
            ? entitiesPropertyInfo
            : entityPropertyInfo;

        IEnumerable<MemberBinding> bindings = [
            Expression.Bind(
                navigationNamePropertyInfo
                , Expression.Constant(navigationName)),
            Expression.Bind(
                isCollectionPropertyInfo
                , Expression.Constant(isCollection)),
            Expression.Bind(
                targetPropertyInfo
                , relatedEntityExpression)
        ];

        var initExpression = Expression.MemberInit(
            Expression.New(ctor)
            , bindings);

        return initExpression;
    }

    private static class RelatedEntityResponseInfo
    {
        public static readonly Type Type = typeof(RelatedEntityResponse);

        public static readonly ConstructorInfo Ctor
            = typeof(RelatedEntityResponse).GetConstructor(Type.EmptyTypes)!;

        public static readonly PropertyInfo NavigationNamePropertyInfo
            = typeof(RelatedEntityResponse).GetProperty(nameof(RelatedEntityResponse.NavigationName))!;

        public static readonly PropertyInfo IsCollectionPropertyInfo
            = typeof(RelatedEntityResponse).GetProperty(nameof(RelatedEntityResponse.IsCollection))!;

        public static readonly PropertyInfo EntityPropertyInfo
            = typeof(RelatedEntityResponse).GetProperty(nameof(RelatedEntityResponse.Entity))!;

        public static readonly PropertyInfo EntitiesPropertyInfo
            = typeof(RelatedEntityResponse).GetProperty(nameof(RelatedEntityResponse.Entities))!;
    }
}
