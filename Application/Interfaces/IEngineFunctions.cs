using Application.Core;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.Interfaces
{
    public interface IEngineFunctions
    {
        public Result<Dictionary<string, object>> ValidateInputData(RuleProjectDto ruleProject, JObject inputData);
        public JObject BuildOutputData(RuleProjectDto ruleProject, Dictionary<string, object> validatedData);
        public Result<JObject> PerformAction(ActionDto action, Dictionary<string, object> inputData, JObject outputObject);
        public Result<bool> EvaluateCondition(ConditionDto condition, Dictionary<string, object> inputData);
    }
}