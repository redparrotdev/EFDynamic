using EFDynamic.Core.Models;

namespace EFDynamic.Core.Extensions;

public static class EntityResponseExtensions
{
    public static Dictionary<string, object?> ToDictionary(this EntityResponse? entityResponse)
    {
        var dict = new Dictionary<string, object?>();
        if (entityResponse is null) return dict;

        foreach (var prop in entityResponse.Properties)
        {
            dict[prop.Name] = prop.Value;
        }
        foreach (var relatedEntity in entityResponse.RelatedEntities)
        {
            object? relatedValue;
            if (relatedEntity.IsCollection)
            {
                relatedValue = relatedEntity.Entities
                    .Select(e => e.ToDictionary())
                    .ToArray();
            }
            else
            {
                relatedValue = relatedEntity.Entity?.ToDictionary();
            }

            dict[relatedEntity.NavigationName] = relatedValue;
        }
        return dict;
    }
}
