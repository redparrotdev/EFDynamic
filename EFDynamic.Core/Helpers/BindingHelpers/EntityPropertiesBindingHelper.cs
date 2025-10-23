using EFDynamic.Core.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace EFDynamic.Core.Helpers;

public static partial class BindingHelper
{
    /// <summary>
    /// Builds an expression that initializes an array of entity property bindings.
    /// </summary>
    /// <remarks>This method creates an expression that constructs an array of objects, each representing a
    /// binding between a property name and its value for a given entity type. Only properties that exist on the
    /// specified entity type are included in the resulting expression.</remarks>
    /// <param name="entityType">The type of the entity whose properties are to be bound.</param>
    /// <param name="entityParameter">The parameter expression representing the entity instance.</param>
    /// <param name="properties">A collection of property names to be included in the bindings.</param>
    /// <returns>An expression representing an array of initialized entity property bindings.</returns>
    public static Expression BuildEntityPropertiesBindings(Type entityType
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

    private static class EntityPropertyModelInfo
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
