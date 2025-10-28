using EFDynamic.Core.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace EFDynamic.Core.Helpers;
public static partial class BindingHelper
{
    /// <summary>
    /// Builds an <see cref="Expression"/> that initializes a new instance of a related entity response object.
    /// </summary>
    /// <remarks>This method constructs a member initialization expression for a related entity response
    /// object. The resulting expression binds the specified navigation name, collection indicator, and related entity
    /// expression to the corresponding properties of the response object. The caller can use the returned expression in
    /// LINQ queries or other expression tree scenarios.</remarks>
    /// <param name="navigationName">The name of the navigation property associated with the related entity.</param>
    /// <param name="isCollection">A value indicating whether the related entity is a collection.  <see langword="true"/> if the related entity is
    /// a collection; otherwise, <see langword="false"/>.</param>
    /// <param name="relatedEntityExpression">An <see cref="Expression"/> representing the related entity or entities to be assigned to the response object.</param>
    /// <returns>An <see cref="Expression"/> that initializes a new instance of the related entity response object with the
    /// specified navigation name,  collection indicator, and related entity expression.</returns>
    public static Expression BuildRelatedEntityResponseInstanceInitExpression(
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

    public static class RelatedEntityResponseInfo
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
