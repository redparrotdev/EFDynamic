using System.Linq.Expressions;

namespace EFDynamic.Core.Helpers;

public static partial class BindingHelper
{
    public static Expression BuildRelatedEntityResponseSingleEntityBinding(
        ParameterExpression parentEntityParameter
        , Type targetEntityType
        , string navigationName
        , Expression responseInitExpression)
    {
        var navigationPropertyAccess = Expression.Property(
            parentEntityParameter
            , navigationName);

        return null!;

    }
}
