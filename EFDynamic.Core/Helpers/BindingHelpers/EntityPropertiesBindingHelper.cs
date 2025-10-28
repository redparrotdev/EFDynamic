using EFDynamic.Core.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace EFDynamic.Core.Helpers;

public static partial class BindingHelper
{
    /// <summary>
    /// Builds an expression that initializes an array of entity property models based on the specified entity type,
    /// entity parameter, and property names.
    /// </summary>
    /// <remarks>This method generates an expression that creates an array of objects, where each object
    /// represents a property of the entity. The resulting expression can be compiled and executed to retrieve the
    /// property values dynamically at runtime. Properties that do not exist on the specified entity type are
    /// ignored.</remarks>
    /// <param name="entityType">The type of the entity whose properties are being processed.</param>
    /// <param name="entityParameter">An expression representing the entity instance from which property values will be retrieved.</param>
    /// <param name="properties">A collection of property names to include in the resulting array. Only properties that exist on the specified
    /// <paramref name="entityType"/> will be included.</param>
    /// <param name="propertyInfoLookup">An optional dictionary mapping property names to their corresponding <see cref="PropertyInfo"/> objects.</param>
    /// <returns>An <see cref="Expression"/> representing the initialization of an array of entity property models. Each model
    /// contains the property name and its corresponding value from the entity instance.</returns>
    public static Expression BuildEntityPropertiesArrayInitExpression(Type entityType
        , Expression entityParameter
        , IEnumerable<string> properties
        , Dictionary<string, PropertyInfo>? propertyInfoLookup = null)
    {
        var ctor = EntityPropertyModelInfo.Ctor;
        var namePropertyInfo = EntityPropertyModelInfo.NamePropertyInfo;
        var valuePropertyInfo = EntityPropertyModelInfo.ValuePropertyInfo;

        var bindings = properties
            .Select(propertyName =>
            {
                var propertyInfo = GetPropertyInfo(propertyName);
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

        // Closure
        PropertyInfo? GetPropertyInfo(string propertyName)
        {
            if (propertyInfoLookup?.TryGetValue(propertyName, out PropertyInfo? propertyInfo) == true)
            {
                return propertyInfo;
            }

            propertyInfo = entityType
                .GetProperty(
                    propertyName
                    , BindingFlags.Public | BindingFlags.Instance);

            return propertyInfo;
        }
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
