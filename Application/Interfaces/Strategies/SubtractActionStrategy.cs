using Application.Core;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.Interfaces.Strategies
{
    public class SubtractActionStrategy : IActionStrategy
    {
        public string ModificationType => "Subtract";
        public Result<JObject> Execute(ActionDto action, Dictionary<string, object> inputData, JObject outputObject)
        {
            JToken targetToken = outputObject.SelectToken(action.TargetProperty);

            if (targetToken == null)
            {
                return Result<JObject>.Failure($"Target field {action.TargetProperty} not found in data.");
            }

            try
            {
                double targetValue = targetToken.Value<double>();
                double modificationValue = double.Parse(action.ModificationValue);

                targetValue -= modificationValue;

                return Result<JObject>.Success(outputObject);
            }
            catch (Exception ex)
            {
                return Result<JObject>.Failure($"Error while modifying field {action.TargetProperty}: {ex.Message}");
            }
        }
    }
}