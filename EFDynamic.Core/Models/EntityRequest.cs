using System.Linq.Expressions;

namespace EFDynamic.Core.Models;

public class EntityRequest
{
    public required string EntityName { get; init; }

    public required LambdaExpression WhereExpression { get; init; }

    public IReadOnlyList<string> Properties { get; init; } = [];

    public IReadOnlyCollection<RelatedEntityRequest> RelatedEntities { get; init; } = [];

    public IReadOnlyCollection<Func<Expression, (string, Expression)>> AdditionalProperties { get; init; } = [];

    public int? Skip { get; init; }
    public int? Take { get; init; }
}

public class RelatedEntityRequest
{
    public required string NavigationName { get; init; }

    public LambdaExpression? WhereExpression { get; init; }

    public IReadOnlyList<string> Properties { get; init; } = [];

    public IReadOnlyCollection<RelatedEntityRequest> RelatedEntities { get; init; } = [];

    public IReadOnlyCollection<Func<Expression, (string, Expression)>> AdditionalProperties { get; init; } = [];

    public int? Skip { get; init; }
    public int? Take { get; init; }
}
