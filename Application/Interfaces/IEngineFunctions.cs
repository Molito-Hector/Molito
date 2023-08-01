using Application.Core;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.Interfaces
{
    public interface IEngineFunctions
    {
        public Result<Dictionary<string, object>> ValidateInputData(RuleWithProjectDto rule, JObject inputData);
        public Result<object> GetValueFromDataInput(Dictionary<string, object> dataInput, string field);
        public JObject BuildOutputData(RuleWithProjectDto rule, Dictionary<string, object> validatedData);
    }
}