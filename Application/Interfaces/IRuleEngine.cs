using Domain;

namespace Application.Interfaces
{
    public interface IRuleEngine
    {
        bool ExecuteRule(RuleDto rule, IDictionary<string, object> data);
    }
}