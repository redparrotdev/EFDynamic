using EFDynamic.Core.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace EFDynamic.Core.Helpers;
public static partial class BindingHelper
{
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
