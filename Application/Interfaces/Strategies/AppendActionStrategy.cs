using Application.Core;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.Interfaces.Strategies
{
    public class AppendActionStrategy : IActionStrategy
    {
        public string ModificationType => "Append";
        public Result<JObject> Execute(ActionDto action, Dictionary<string, object> inputData, JObject outputObject)
        {
            JToken targetToken = outputObject.SelectToken(action.TargetProperty);

            if (targetToken == null)
            {
                return Result<JObject>.Failure($"Target field {action.TargetProperty} not found in data.");
            }

            try
            {
                if (targetToken.Type == JTokenType.String)
                {
                    targetToken.Replace((string)targetToken + action.ModificationValue);

                    return Result<JObject>.Success(outputObject);
                }
                else
                {
                    return Result<JObject>.Failure($"Invalid operation {action.ModificationType} on non-string type.");
                }
            }
            catch (Exception ex)
            {
                return Result<JObject>.Failure($"Error while modifying field {action.TargetProperty}: {ex.Message}");
            }
        }
    }
}