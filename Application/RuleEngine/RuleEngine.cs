using Application.Core;
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
        public Result<bool> ExecuteRule(RuleDto rule, JObject data)
        {
            if (rule == null)
            {
                return Result<bool>.Failure("Rule not found or not provided");
            }

            var validatedData = ValidateInputData(rule, data);
            if (!validatedData.IsSuccess) return Result<bool>.Failure(validatedData.Error);

            foreach (var conditiondto in rule.Conditions)
            {
                Condition condition = _mapper.Map<Condition>(conditiondto);

                var evaluation = EvaluateCondition(condition, validatedData.Value);
                if (!evaluation.IsSuccess) return Result<bool>.Failure(evaluation.Error);

                if (!evaluation.Value)
                {
                    return Result<bool>.Success(false);
                }
            }

            foreach (var action in rule.Actions)
            {
                PerformAction(action, data);
            }

            return Result<bool>.Success(true);
        }

        private Result<bool> EvaluateCondition(Condition condition, Dictionary<string, object> inputData)
        {
            if (condition.SubConditions.Count > 0)
            {
                var results = condition.SubConditions.Select(subCondition => EvaluateCondition(subCondition, inputData).Value);
                return Result<bool>.Success(condition.LogicalOperator.ToLower() == "and" ? results.All(x => x) : results.Any(x => x));
            }

            var fieldData = GetValueFromDataInput(inputData, condition.Field);
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

        private void PerformAction(ActionDto action, JObject data)
        {
            Console.WriteLine($"Performing action: {action.Name}");
        }

        private Result<Dictionary<string, object>> ValidateInputData(RuleDto rule, JObject inputData)
        {
            var validatedData = new Dictionary<string, object>();

            foreach (var inputProperty in rule.Properties.Where(x => x.Direction == "I" || x.Direction == "B"))
            {
                JToken token;
                if (!inputData.TryGetValue(inputProperty.Name, out token))
                {
                    return Result<Dictionary<string, object>>.Failure($"Missing required property {inputProperty.Name}");
                }

                var convertedValue = ValidateAndConvert(token, _mapper.Map<RuleProperty>(inputProperty).Type, inputProperty.SubProperties);
                if (!convertedValue.IsSuccess) return Result<Dictionary<string, object>>.Failure(convertedValue.Error);

                validatedData[inputProperty.Name] = convertedValue.Value;
            }

            return Result<Dictionary<string, object>>.Success(validatedData);
        }

        private Result<object> ValidateAndConvert(JToken token, PropertyType type, ICollection<SubpropertyDto> subProperties)
        {
            switch (type)
            {
                case PropertyType.StringType:
                    if (token.Type != JTokenType.String)
                    {
                        return Result<object>.Failure($"Expected String but got {token.Type}");
                    }
                    return Result<object>.Success(token.Value<string>());

                case PropertyType.IntegerType:
                    if (token.Type != JTokenType.Integer)
                    {
                        return Result<object>.Failure($"Expected Integer but got {token.Type}");
                    }
                    return Result<object>.Success(token.Value<int>());

                case PropertyType.BooleanType:
                    if (token.Type != JTokenType.Boolean)
                    {
                        return Result<object>.Failure($"Expected Boolean but got {token.Type}");
                    }
                    return Result<object>.Success(token.Value<bool>());

                case PropertyType.ObjectType:
                    if (token.Type != JTokenType.Object)
                    {
                        return Result<object>.Failure($"Expected Object but got {token.Type}");
                    }

                    var result = new Dictionary<string, object>();
                    var inputData = (JObject)token;

                    foreach (var subProperty in subProperties)
                    {
                        JToken subToken;
                        if (!inputData.TryGetValue(subProperty.Name, out subToken))
                        {
                            return Result<object>.Failure($"Missing required sub-property {subProperty.Name}");
                        }

                        var convertedValue = ValidateAndConvert(subToken, _mapper.Map<RuleProperty>(subProperty).Type, null);
                        if (!convertedValue.IsSuccess) return Result<object>.Failure(convertedValue.Error);

                        result[subProperty.Name] = convertedValue.Value;
                    }

                    return Result<object>.Success(result);

                default:
                    return Result<object>.Failure("Invalid property type");
            }
        }

        private Result<object> GetValueFromDataInput(Dictionary<string, object> dataInput, string field)
        {
            var properties = field.Split('.');

            object currentObject = dataInput;

            foreach (var property in properties)
            {
                if (!(currentObject is Dictionary<string, object> dict))
                {
                    return Result<object>.Failure($"Invalid field {field}: {property} is not an object");
                }

                if (!dict.TryGetValue(property, out currentObject))
                {
                    return Result<object>.Failure($"Invalid field {field}: {property} not found in input data");
                }
            }

            return Result<object>.Success(currentObject);
        }
    }
}
