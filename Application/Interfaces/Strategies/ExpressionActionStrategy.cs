using Application.Core;
using Domain;
using MathNet.Symbolics;
using Newtonsoft.Json.Linq;

namespace Application.Interfaces.Strategies
{
    public class ExpressionActionStrategy : IActionStrategy
    {
        public string ModificationType => "Expression";
        private readonly IEngineFunctions _engineFunctions;
        public ExpressionActionStrategy(IEngineFunctions engineFunctions)
        {
            _engineFunctions = engineFunctions;
        }

        public Result<JObject> Execute(ActionDto action, Dictionary<string, object> inputData, JObject outputObject)
        {
            JToken targetToken = outputObject.SelectToken(action.TargetProperty);

            if (targetToken == null)
            {
                return Result<JObject>.Failure($"Target field {action.TargetProperty} not found in data.");
            }

            try
            {
                var variables = new Dictionary<string, FloatingPoint>();

                var start = action.ModificationValue.IndexOf('{');
                while (true)
                {
                    int end = action.ModificationValue.IndexOf('}', start);
                    if (end == -1)
                    {
                        return Result<JObject>.Failure("Unmatched bracket in modification value");
                    }

                    string propertyName = action.ModificationValue.Substring(start + 1, end - start - 1);

                    var valueRetrieval = _engineFunctions.GetValueFromDataInput(inputData, propertyName);
                    if (!valueRetrieval.IsSuccess) return Result<JObject>.Failure(valueRetrieval.Error);

                    var propertyValue = (double)valueRetrieval.Value;

                    string varName = propertyName.Replace(".", "_");

                    action.ModificationValue = action.ModificationValue.Replace("{" + propertyName + "}", varName);

                    variables.Add(varName, propertyValue);

                    var moreCheck = action.ModificationValue.Contains("{");
                    if (!moreCheck)
                    {
                        break;
                    }
                    start = action.ModificationValue.IndexOf('{', end);
                }

                Expression e;

                try
                {
                    e = Infix.ParseOrThrow(action.ModificationValue);
                }
                catch (Exception ex)
                {
                    return Result<JObject>.Failure($"Invalid modification value: {ex.Message}");
                }

                double result = Evaluate.Evaluate(variables, e).RealValue;
                targetToken.Replace(JToken.FromObject(result));

                return Result<JObject>.Success(outputObject);
            }
            catch (Exception ex)
            {
                return Result<JObject>.Failure($"Error while modifying field {action.TargetProperty}: {ex.Message}");
            }
        }
    }
}