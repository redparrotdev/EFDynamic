using EFDynamic.Core.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace EFDynamic.Core.Helpers;

public static partial class BindingHelper
{
    public static Expression BuildEntityPropertiesArrayInitExpression(Type entityType
        , Expression entityParameter
        , IEnumerable<string> properties)
    {
        var ctor = EntityPropertyModelInfo.Ctor;
        var namePropertyInfo = EntityPropertyModelInfo.NamePropertyInfo;
        var valuePropertyInfo = EntityPropertyModelInfo.ValuePropertyInfo;

        var bindings = properties
            .Select(propertyName =>
            {
                // TODO: use context model to fetch data faster
                var propertyInfo = entityType
                    .GetProperty(
                        propertyName
                        , BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo is null) return null!;

                return Expression.MemberInit(
                    Expression.New(ctor)
                    , Expression.Bind(
                        namePropertyInfo
                        , Expression.Constant(propertyInfo.Name))
                    , Expression.Bind(
                        valuePropertyInfo
                        , Expression.Convert(
                            Expression.Property(entityParameter, propertyInfo)
                            , typeof(object))));
            })
            .Where(bindings => bindings is not null)
            .ToArray();

        var arrayInit = Expression.NewArrayInit(
            EntityPropertyModelInfo.Type
            , bindings);

        return arrayInit;
    }

    public static Expression BuildEntityPropertyInstanceInitExpression(
        string name
        , Expression valueExpression)
    {
        var ctor = EntityPropertyModelInfo.Ctor;
        var namePropertyInfo = EntityPropertyModelInfo.NamePropertyInfo;
        var valuePropertyInfo = EntityPropertyModelInfo.ValuePropertyInfo;

        var binding = Expression.MemberInit(
            Expression.New(ctor)
            , Expression.Bind(
                namePropertyInfo
                , Expression.Constant(name))
            , Expression.Bind(
                valuePropertyInfo
                , Expression.Convert(
                    valueExpression
                    , typeof(object))));

        return binding;
    }

    public static class EntityPropertyModelInfo
    {
        public static readonly Type Type = typeof(EntityPropertyModel);

        public static readonly ConstructorInfo Ctor
            = typeof(EntityPropertyModel).GetConstructor(Type.EmptyTypes)!;

        public static readonly PropertyInfo NamePropertyInfo
            = typeof(EntityPropertyModel).GetProperty(nameof(EntityPropertyModel.Name))!;

        public static readonly PropertyInfo ValuePropertyInfo
            = typeof(EntityPropertyModel).GetProperty(nameof(EntityPropertyModel.Value))!;
    }
}
