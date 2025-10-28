using System.Linq.Expressions;

namespace EFDynamic.Core.Utility;

public class ParametersFactory
{
    private readonly string _parameterTemplate;
    private readonly Dictionary<string, ParameterExpression> _parametersLookup = new();

    private int _nextParameterIndex = 0;

    public ParametersFactory(string parameterTemplate = "p_{0}")
    {
        _parameterTemplate = parameterTemplate;
    }

    public string NextParameterName()
    {
        return string
            .Format(_parameterTemplate, _nextParameterIndex++);
    }

    public ParameterExpression CreateParameter(Type entityType)
    {
        var parameterName = NextParameterName();
        var parameter = Expression.Parameter(
            entityType
            , parameterName);
        _parametersLookup[parameterName] = parameter;

        return parameter;
    }

    public bool TryFindParameterByName(string parameterName, out ParameterExpression? parameter)
    {
        return _parametersLookup.TryGetValue(
            parameterName
            , out parameter);
    }
}
