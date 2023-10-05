using Application.Core;
using Application.Interfaces;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.RuleEngine
{
    public class RuleEngine : IRuleEngine
    {
        private readonly IEngineFunctions _engineFunctions;
        public RuleEngine(IEngineFunctions engineFunctions)
        {
            _engineFunctions = engineFunctions;
        }
        public Result<JObject> ExecuteRule(RuleWithProjectDto rule, JObject data)
        {
            if (rule == null)
            {
                return Result<JObject>.Failure("Rule not found or not provided");
            }

            var validatedData = _engineFunctions.ValidateInputData(rule.RuleProject, data);
            if (!validatedData.IsSuccess) return Result<JObject>.Failure(validatedData.Error);

            var outputData = _engineFunctions.BuildOutputData(rule.RuleProject, validatedData.Value);

            if (rule.Conditions.Count == 0)
            {
                data.Add("Molito Message", "Rule doesn't contain any conditions");
                return Result<JObject>.Success(data);
            }

            foreach (var conditiondto in rule.Conditions)
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

        public Result<JObject> ExecuteTable(DTWithProjectDto table, JObject data)
        {
            if (table == null)
                return Result<JObject>.Failure("Decision Table not found or not provided");

            var validatedData = _engineFunctions.ValidateInputData(table.RuleProject, data);
            if (!validatedData.IsSuccess) return Result<JObject>.Failure(validatedData.Error);

            var outputData = _engineFunctions.BuildOutputData(table.RuleProject, validatedData.Value);

            if (table.Conditions.Count == 0)
            {
                data.Add("Molito Message", "Decision Table doesn't contain any conditions");
                return Result<JObject>.Success(data);
            }

            var matchCount = 0;
            foreach (DecisionRowDto row in table.Rows)
            {
                var conditionMatch = EvaluateRowConditions(row, table.Conditions, validatedData.Value);
                if (!conditionMatch.IsSuccess) return Result<JObject>.Failure(conditionMatch.Error);

                if (!conditionMatch.Value) continue;

                foreach (var action in table.Actions)
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
