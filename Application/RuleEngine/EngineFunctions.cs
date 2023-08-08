using Application.Core;
using Application.Interfaces;
using Application.Interfaces.Strategies;
using AutoMapper;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.RuleEngine
{
    public class EngineFunctions : IEngineFunctions
    {
        private readonly IMapper _mapper;
        private readonly ActionStrategyFactory _actionStrategyFactory;
        public EngineFunctions(IMapper mapper, ActionStrategyFactory actionStrategyFactory)
        {
            _actionStrategyFactory = actionStrategyFactory;
            _mapper = mapper;
        }

        public Result<Dictionary<string, object>> ValidateInputData(RuleProjectDto ruleProject, JObject inputData)
        {
            var validatedData = new Dictionary<string, object>();
            var inputProperties = ruleProject.Properties.Where(x => x.Direction == "I" || x.Direction == "B");

            foreach (var inputProperty in inputProperties)
            {
                if (!inputData.TryGetValue(inputProperty.Name, out JToken token))
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

                case PropertyType.NumberType:
                    if (token.Type != JTokenType.Float)
                    {
                        return Result<object>.Failure($"Expected Decimal Number but got {token.Type}");
                    }
                    return Result<object>.Success(token.Value<double>());

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
                        if (!inputData.TryGetValue(subProperty.Name, out JToken subToken))
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
                if (currentObject is not Dictionary<string, object> dict)
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

        public JObject BuildOutputData(RuleProjectDto ruleProject, Dictionary<string, object> validatedData)
        {
            var outputData = new JObject();
            var outputProperties = ruleProject.Properties.Where(rp => rp.Direction == "O" || rp.Direction == "B");

            foreach (var property in outputProperties)
            {
                RuleProperty currentProperty = _mapper.Map<RuleProperty>(property);

                if (currentProperty.Type == PropertyType.ObjectType && currentProperty.SubProperties.Count > 0)
                {
                    outputData[currentProperty.Name] = BuildNestedOutputData(currentProperty, validatedData);
                }
                else
                {
                    var retrieveValue = GetValueFromDataInput(validatedData, currentProperty.Name);
                    object value;
                    if (retrieveValue.IsSuccess)
                    {
                        value = retrieveValue.Value;
                    }
                    else
                    {
                        value = GetDefaultValue(currentProperty.Type);
                    }
                    outputData[currentProperty.Name] = JToken.FromObject(value);
                }
            }

            return outputData;
        }

        private JObject BuildNestedOutputData(RuleProperty currentProperty, Dictionary<string, object> validatedData)
        {
            var outputData = new JObject();
            string propertyNamePrefix = currentProperty.Name + ".";

            foreach (var subProperty in currentProperty.SubProperties)
            {
                RuleProperty currentSubProperty = _mapper.Map<RuleProperty>(subProperty);

                string fullPropertyName = propertyNamePrefix + currentSubProperty.Name;
                if (currentSubProperty.Type == PropertyType.ObjectType && currentSubProperty.SubProperties.Count > 0)
                {
                    outputData[currentSubProperty.Name] = BuildNestedOutputData(currentSubProperty, validatedData);
                }
                else
                {
                    var retrieveValue = GetValueFromDataInput(validatedData, fullPropertyName);
                    object value;
                    if (retrieveValue.IsSuccess)
                    {
                        value = retrieveValue.Value;
                    }
                    else
                    {
                        value = GetDefaultValue(currentSubProperty.Type);
                    }
                    outputData[currentSubProperty.Name] = JToken.FromObject(value);
                }
            }

            return outputData;
        }

        public Result<JObject> PerformAction(ActionDto action, Dictionary<string, object> inputData, JObject outputObject)
        {
            IActionStrategy strategy;

            try
            {
                strategy = _actionStrategyFactory.CreateStrategy(action.ModificationType);
            }
            catch (ArgumentException e)
            {
                return Result<JObject>.Failure(e.Message);
            }

            return strategy.Execute(action, inputData, outputObject);
        }

        public Result<bool> EvaluateCondition(ConditionDto condition, Dictionary<string, object> inputData)
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

        private Result<bool> EvaluateSubConditions(ConditionDto condition, Dictionary<string, object> inputData)
        {
            switch (condition.LogicalOperator.ToLower())
            {
                case "and":
                    foreach (var subCondition in condition.SubConditions)
                    {
                        var result = EvaluateCondition(subCondition, inputData);
                        if (!result.IsSuccess) return Result<bool>.Failure(result.Error);
                        if (!result.Value) return Result<bool>.Success(false);  // If one condition is false, return false for "AND"
                    }
                    return Result<bool>.Success(true);

                case "or":
                    foreach (var subCondition in condition.SubConditions)
                    {
                        var result = EvaluateCondition(subCondition, inputData);
                        if (!result.IsSuccess) return Result<bool>.Failure(result.Error);
                        if (result.Value) return Result<bool>.Success(true);  // If one condition is true, return true for "OR"
                    }
                    return Result<bool>.Success(false);

                default:
                    return Result<bool>.Failure($"Invalid logical operator {condition.LogicalOperator}");
            }
        }

        private Result<bool> EvaluateComparison(ConditionDto condition, Dictionary<string, object> inputData)
        {
            var fieldData = GetValueFromDataInput(inputData, condition.Field);
            if (!fieldData.IsSuccess) return Result<bool>.Failure(fieldData.Error);

            var fieldInData = fieldData.Value;

            return condition.Operator switch
            {
                ">" => Result<bool>.Success(Convert.ToDouble(fieldInData) > Convert.ToDouble(condition.Value)),
                "<" => Result<bool>.Success(Convert.ToDouble(fieldInData) < Convert.ToDouble(condition.Value)),
                ">=" => Result<bool>.Success(Convert.ToDouble(fieldInData) >= Convert.ToDouble(condition.Value)),
                "<=" => Result<bool>.Success(Convert.ToDouble(fieldInData) <= Convert.ToDouble(condition.Value)),
                "==" => Result<bool>.Success(fieldInData.ToString() == condition.Value),
                "!=" => Result<bool>.Success(fieldInData.ToString() != condition.Value),
                _ => Result<bool>.Failure($"Invalid operator {condition.Operator}"),
            };
        }

        private object GetDefaultValue(PropertyType type)
        {
            return type switch
            {
                PropertyType.StringType => string.Empty,
                PropertyType.NumberType => 0.0,
                PropertyType.BooleanType => false,
                PropertyType.ObjectType => new JObject(),
                _ => throw new ArgumentException($"Invalid property type: {type}"),
            };
        }
    }
}