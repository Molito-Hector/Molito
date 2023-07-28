using Application.Core;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.Interfaces.Strategies
{
    public class SetActionStrategy : IActionStrategy
    {
        public string ModificationType => "Set";
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
                    targetToken.Replace(action.ModificationValue);
                }
                else if (targetToken.Type == JTokenType.Float)
                {
                    targetToken.Replace(Convert.ToDouble(action.ModificationValue));
                }

                return Result<JObject>.Success(outputObject);
            }
            catch (Exception ex)
            {
                return Result<JObject>.Failure($"Error while modifying field {action.TargetProperty}: {ex.Message}");
            }
        }
    }
}