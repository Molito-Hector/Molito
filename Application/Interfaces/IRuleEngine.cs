using Domain;
using Newtonsoft.Json.Linq;

namespace Application.Interfaces
{
    public interface IRuleEngine
    {
        bool ExecuteRule(RuleDto rule, JObject data);
    }
}