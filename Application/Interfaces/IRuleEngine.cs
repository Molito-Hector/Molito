using Application.Core;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.Interfaces
{
    public interface IRuleEngine
    {
        Result<bool> ExecuteRule(RuleDto rule, JObject data);
    }
}