using Application.Core;
using Application.Interfaces;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.RuleFlows
{
    public class DecisionTableFlowElement : IFlowElement
    {
        private readonly IEngineFunctions _engineFunctions;
        public DecisionTableFlowElement(IEngineFunctions engineFunctions)
        {
            _engineFunctions = engineFunctions;
        }

        public DTWithProjectDto DecisionTable { get; set; }

        public Result<JObject> Execute(JObject data)
        {
            if (DecisionTable == null)
                return Result<JObject>.Failure("Decision Table not found or not provided");

            var validatedData = _engineFunctions.ValidateInputData(DecisionTable.RuleProject, data);
            if (!validatedData.IsSuccess) return Result<JObject>.Failure(validatedData.Error);

            var outputData = _engineFunctions.BuildOutputData(DecisionTable.RuleProject, validatedData.Value);

            if (DecisionTable.Conditions.Count == 0)
            {
                data.Add("Molito Message", "Decision Table doesn't contain any conditions");
                return Result<JObject>.Success(data);
            }

            var matchCount = 0;
            foreach (DecisionRowDto row in DecisionTable.Rows)
            {
                var conditionMatch = EvaluateRowConditions(row, DecisionTable.Conditions, validatedData.Value);
                if (!conditionMatch.IsSuccess) return Result<JObject>.Failure(conditionMatch.Error);

                if (!conditionMatch.Value) continue;

                foreach (var action in DecisionTable.Actions)
                {
                    matchCount++;
                    action.ModificationValue = row.ActionValues.Where(a => a.ActionId == action.Id).FirstOrDefault().Value;
                    var result = _engineFunctions.PerformAction(action, validatedData.Value, outputData);
                    if (!result.IsSuccess) return Result<JObject>.Failure(result.Error);
                }
            }

            if (matchCount == 0)
            {
                data.Add("Molito Message", "Execution ended without triggering a condition");
                return Result<JObject>.Success(data);
            }

            return Result<JObject>.Success(outputData);
        }

        private Result<bool> EvaluateRowConditions(DecisionRowDto row, ICollection<ConditionDto> tableConditions, Dictionary<string, object> validatedData)
        {
            var conditionMap = tableConditions.ToDictionary(c => c.Id);

            foreach (ConditionValueDto conditionValue in row.Values)
            {
                conditionMap.TryGetValue(conditionValue.ConditionId, out var condition);

                if (condition == null) return Result<bool>.Failure("No corresponding condition found in the table");

                condition.Value = conditionValue.Value;
                var evaluation = _engineFunctions.EvaluateCondition(condition, validatedData);
                if (!evaluation.IsSuccess) return Result<bool>.Failure(evaluation.Error);

                if (!evaluation.Value) return Result<bool>.Success(evaluation.Value);
            }

            return Result<bool>.Success(true);
        }
    }
}