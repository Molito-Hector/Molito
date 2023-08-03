using Application.Core;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.Interfaces
{
    public interface IEngineFunctions
    {
        public Result<Dictionary<string, object>> ValidateInputData(RuleProjectDto ruleProject, JObject inputData);
        public Result<object> GetValueFromDataInput(Dictionary<string, object> dataInput, string field);
        public JObject BuildOutputData(RuleProjectDto ruleProject, Dictionary<string, object> validatedData);
    }
}