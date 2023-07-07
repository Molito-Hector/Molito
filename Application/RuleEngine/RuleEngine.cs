using Application.Interfaces;
using AutoMapper;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.RuleEngine
{
    public class RuleEngine : IRuleEngine
    {
        private readonly IMapper _mapper;
        public RuleEngine(IMapper mapper)
        {
            _mapper = mapper;
        }
        public bool ExecuteRule(RuleDto rule, JObject data)
        {
            if (rule == null)
            {
                return false;
            }

            foreach (var conditiondto in rule.Conditions)
            {
                Condition condition = _mapper.Map<Condition>(conditiondto);

                if (!EvaluateCondition(condition, data))
                {
                    return false;
                }
            }

            foreach (var action in rule.Actions)
            {
                PerformAction(action, data);
            }

            return true;
        }

        public bool EvaluateCondition(Condition condition, JObject inputData)
        {
            if (condition.SubConditions.Count > 0)
            {
                var results = condition.SubConditions.Select(subCondition => EvaluateCondition(subCondition, inputData));
                return condition.LogicalOperator.ToLower() == "and" ? results.All(x => x) : results.Any(x => x);
            }

            var fieldInData = inputData[condition.Field];

            if (fieldInData == null)
            {
                throw new Exception($"Field {condition.Field} not found in input data.");
            }

            switch (condition.Operator)
            {
                case ">":
                    return Convert.ToDouble(fieldInData) > Convert.ToDouble(condition.Value);
                case "<":
                    return Convert.ToDouble(fieldInData) < Convert.ToDouble(condition.Value);
                case ">=":
                    return Convert.ToDouble(fieldInData) >= Convert.ToDouble(condition.Value);
                case "<=":
                    return Convert.ToDouble(fieldInData) <= Convert.ToDouble(condition.Value);
                case "==":
                    return fieldInData.ToString() == condition.Value;
                case "!=":
                    return fieldInData.ToString() != condition.Value;
                default:
                    throw new Exception($"Invalid operator {condition.Operator}");
            }
        }

        private void PerformAction(ActionDto action, JObject data)
        {
            Console.WriteLine($"Performing action: {action.Name}");
        }
    }
}