using Application.Core;
using Application.Interfaces;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.RuleFlows
{
    public class RuleFlowElement : IFlowElement
    {
        private readonly IEngineFunctions _engineFunctions;
        public RuleFlowElement(IEngineFunctions engineFunctions)
        {
            _engineFunctions = engineFunctions;
        }

        public RuleWithProjectDto Rule { get; set; }
        
        public Result<JObject> Execute(JObject data)
        {
            if (Rule == null)
            {
                return Result<JObject>.Failure("Rule not found or not provided");
            }

            var validatedData = _engineFunctions.ValidateInputData(Rule.RuleProject, data);
            if (!validatedData.IsSuccess) return Result<JObject>.Failure(validatedData.Error);

            var outputData = _engineFunctions.BuildOutputData(Rule.RuleProject, validatedData.Value);

            if (Rule.Conditions.Count == 0)
            {
                data.Add("Molito Message", "Rule doesn't contain any conditions");
                return Result<JObject>.Success(data);
            }

            foreach (var conditiondto in Rule.Conditions)
            {
                var evaluation = _engineFunctions.EvaluateCondition(conditiondto, validatedData.Value);
                if (!evaluation.IsSuccess) return Result<JObject>.Failure(evaluation.Error);

                if (!evaluation.Value)
                {
                    data.Add("Molito Message", "Execution ended without triggering a condition");
                    return Result<JObject>.Success(data);
                }

                foreach (var action in conditiondto.Actions)
                {
                    var result = _engineFunctions.PerformAction(action, validatedData.Value, outputData);
                    if (!result.IsSuccess) return Result<JObject>.Failure(result.Error);
                }
            }

            return Result<JObject>.Success(outputData);
        }
    }
}