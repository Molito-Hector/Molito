using Application.Core;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.Interfaces
{
    public interface IRuleEngine
    {
        Result<JObject> ExecuteRule(RuleDto rule, JObject data);
    }
}