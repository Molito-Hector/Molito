using Application.Core;
using Application.Interfaces;
using Application.Interfaces.Strategies;
using AutoMapper;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.RuleEngine
{
    public class RuleEngine : IRuleEngine
    {
        private readonly IMapper _mapper;
        private readonly IEngineFunctions _engineFunctions;
        private readonly ActionStrategyFactory _actionStrategyFactory;
        public RuleEngine(IMapper mapper, IEngineFunctions engineFunctions, ActionStrategyFactory actionStrategyFactory)
        {
            _actionStrategyFactory = actionStrategyFactory;
            _mapper = mapper;
            _engineFunctions = engineFunctions;
        }
        public Result<JObject> ExecuteRule(RuleDto rule, JObject data)
        {
            if (rule == null)
            {
                return Result<JObject>.Failure("Rule not found or not provided");
            }

            var validatedData = _engineFunctions.ValidateInputData(rule, data);
            if (!validatedData.IsSuccess) return Result<JObject>.Failure(validatedData.Error);

            foreach (var conditiondto in rule.Conditions)
            {
                Condition condition = _mapper.Map<Condition>(conditiondto);

                var evaluation = EvaluateCondition(condition, validatedData.Value);
                if (!evaluation.IsSuccess) return Result<JObject>.Failure(evaluation.Error);

                if (!evaluation.Value)
                {
                    data.Add("Molito Message", "Execution ended without triggering a condition");
                    return Result<JObject>.Success(data);
                }
            }

            var outputData = _engineFunctions.BuildOutputData(rule, validatedData.Value);

            foreach (var action in rule.Actions)
            {
                var result = PerformAction(action, validatedData.Value, outputData);
                if (!result.IsSuccess) return Result<JObject>.Failure(result.Error);
            }

            return Result<JObject>.Success(outputData);
        }

        private Result<bool> EvaluateCondition(Condition condition, Dictionary<string, object> inputData)
        {
            if (condition == null) return Result<bool>.Failure("Condition is null");

            if (condition.SubConditions.Count > 0)
            {
                return EvaluateSubConditions(condition, inputData);
            }
            else
            {
                return EvaluateComparison(condition, inputData);
            }
        }

        private Result<bool> EvaluateSubConditions(Condition condition, Dictionary<string, object> inputData)
        {
            var results = condition.SubConditions.Select(subCondition => EvaluateCondition(subCondition, inputData).Value);
            return Result<bool>.Success(condition.LogicalOperator.ToLower() == "and" ? results.All(x => x) : results.Any(x => x));
        }

        private Result<bool> EvaluateComparison(Condition condition, Dictionary<string, object> inputData)
        {
            var fieldData = _engineFunctions.GetValueFromDataInput(inputData, condition.Field);
            if (!fieldData.IsSuccess) return Result<bool>.Failure(fieldData.Error);

            var fieldInData = fieldData.Value;

            switch (condition.Operator)
            {
                case ">":
                    return Result<bool>.Success(Convert.ToDouble(fieldInData) > Convert.ToDouble(condition.Value));
                case "<":
                    return Result<bool>.Success(Convert.ToDouble(fieldInData) < Convert.ToDouble(condition.Value));
                case ">=":
                    return Result<bool>.Success(Convert.ToDouble(fieldInData) >= Convert.ToDouble(condition.Value));
                case "<=":
                    return Result<bool>.Success(Convert.ToDouble(fieldInData) <= Convert.ToDouble(condition.Value));
                case "==":
                    return Result<bool>.Success(fieldInData.ToString() == condition.Value);
                case "!=":
                    return Result<bool>.Success(fieldInData.ToString() != condition.Value);
                default:
                    return Result<bool>.Failure($"Invalid operator {condition.Operator}");
            }
        }

        private Result<JObject> PerformAction(ActionDto action, Dictionary<string, object> inputData, JObject outputObject)
        {
            IActionStrategy strategy;

            try
            {
                strategy = _actionStrategyFactory.CreateStrategy(action.ModificationType.ToString());
            }
            catch (ArgumentException e)
            {
                return Result<JObject>.Failure(e.Message);
            }

            return strategy.Execute(action, inputData, outputObject);
        }
    }
}
