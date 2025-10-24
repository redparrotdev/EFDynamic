namespace EFDynamic.Core.Models;

public class EntityResponse
{
    public EntityPropertyModel[] Properties { get; set; } = [];
    public RelatedEntityResponse[] RelatedEntities { get; set; } = [];

    public EntityPropertyModel[] AdditionalProperties { get; set; } = [];
}

public class RelatedEntityResponse
{
    public string NavigationName { get; set; } = string.Empty;
    public bool IsCollection { get; set; }

    public EntityResponse? Entity { get; set; }
    public EntityResponse[] Entities { get; set; } = [];
}
