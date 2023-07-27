using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MathNet.Symbolics;
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
        public Result<JObject> ExecuteRule(RuleDto rule, JObject data)
        {
            if (rule == null)
            {
                return Result<JObject>.Failure("Rule not found or not provided");
            }

            var validatedData = ValidateInputData(rule, data);
            if (!validatedData.IsSuccess) return Result<JObject>.Failure(validatedData.Error);

            foreach (var conditiondto in rule.Conditions)
            {
                Condition condition = _mapper.Map<Condition>(conditiondto);

                var evaluation = EvaluateCondition(condition, validatedData.Value);
                if (!evaluation.IsSuccess) return Result<JObject>.Failure(evaluation.Error);

                if (!evaluation.Value)
                {
                    return Result<JObject>.Success(data);
                }
            }

            var outputData = BuildOutputData(rule, validatedData.Value);

            // foreach (var property in rule.Properties.Where(rp => rp.Direction == "O" || rp.Direction == "B"))
            // {
            //     RuleProperty currentProperty = _mapper.Map<RuleProperty>(property);
            //     var value = validatedData.Value.ContainsKey(currentProperty.Name)
            //         ? validatedData.Value[currentProperty.Name]
            //         : GetDefaultValue(currentProperty.Type);
            //     outputData[currentProperty.Name] = JToken.FromObject(value);
            // }

            foreach (var action in rule.Actions)
            {
                var result = PerformAction2(action, validatedData.Value, outputData);
                if (!result.IsSuccess) return Result<JObject>.Failure(result.Error);
            }

            return Result<JObject>.Success(outputData);
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

        private Result<JObject> PerformAction(ActionDto action, JObject data)
        {
            if (!string.IsNullOrEmpty(action.TargetProperty))
            {
                JToken targetToken = data.SelectToken(action.TargetProperty);

                if (targetToken == null)
                {
                    return Result<JObject>.Failure($"Target field {action.TargetProperty} not found in data.");
                }

                try
                {
                    switch (action.ModificationType)
                    {
                        case ModType.Set:
                            if (targetToken.Type == JTokenType.String)
                            {
                                targetToken.Replace(action.ModificationValue);
                            }
                            else if (targetToken.Type == JTokenType.Float)
                            {
                                targetToken.Replace(Convert.ToDouble(action.ModificationValue));
                            }
                            break;
                        case ModType.Add:
                        case ModType.Subtract:
                        case ModType.Multiply:
                        case ModType.Divide:
                            double targetValue = targetToken.Value<double>();
                            double modificationValue = double.Parse(action.ModificationValue);

                            switch (action.ModificationType)
                            {
                                case ModType.Add:
                                    targetValue += modificationValue;
                                    break;
                                case ModType.Subtract:
                                    targetValue -= modificationValue;
                                    break;
                                case ModType.Multiply:
                                    targetValue *= modificationValue;
                                    break;
                                case ModType.Divide:
                                    targetValue /= modificationValue;
                                    break;
                            }

                            if (targetToken.Type == JTokenType.Float)
                            {
                                targetToken.Replace(Convert.ToDouble(targetValue));
                            }
                            else
                            {
                                targetToken.Replace(targetValue);
                            }
                            break;
                        case ModType.Expression:
                            var variables = new Dictionary<string, FloatingPoint>();

                            var start = action.ModificationValue.IndexOf('{');
                            var end = action.ModificationValue.IndexOf('}');
                            var propertyName = action.ModificationValue.Substring(start + 1, end - start - 1);

                            var expressionValue = action.ModificationValue.Replace("{" + propertyName + "}", propertyName);

                            var token = data.SelectToken(propertyName);
                            double propertyValue;
                            if (token != null && token.Type == JTokenType.Float)
                            {
                                propertyValue = token.Value<double>();
                            }
                            else
                            {
                                propertyValue = 1.5;
                            }

                            variables.Add(propertyName, propertyValue);

                            Expression e;

                            try
                            {
                                e = Infix.ParseOrThrow(expressionValue);
                            }
                            catch (Exception ex)
                            {
                                return Result<JObject>.Failure($"Invalid modification value: {ex.Message}");
                            }

                            double result = Evaluate.Evaluate(variables, e).RealValue;
                            targetToken.Replace(JToken.FromObject(result));

                            break;
                        case ModType.Append:
                        case ModType.Prepend:
                            if (targetToken.Type == JTokenType.String)
                            {
                                targetToken.Replace(
                                    action.ModificationType == ModType.Append
                                        ? ((string)targetToken + action.ModificationValue)
                                        : (action.ModificationValue + (string)targetToken));
                            }
                            else
                            {
                                return Result<JObject>.Failure($"Invalid operation {action.ModificationType} on non-string type.");
                            }
                            break;
                        default:
                            return Result<JObject>.Failure($"Invalid modification type {action.ModificationType}.");
                    }
                }
                catch (Exception ex)
                {
                    return Result<JObject>.Failure($"Error while modifying field {action.TargetProperty}: {ex.Message}");
                }
            }

            return Result<JObject>.Success(data);
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
            var properties = field.Split('_');

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

        private Result<JObject> PerformAction2(ActionDto action, Dictionary<string, object> inputData, JObject outputObject)
        {
            if (!string.IsNullOrEmpty(action.TargetProperty))
            {
                string jsonPath = action.TargetProperty.Replace("_", ".");
                JToken targetToken = outputObject.SelectToken(jsonPath);

                if (targetToken == null)
                {
                    return Result<JObject>.Failure($"Target field {action.TargetProperty} not found in data.");
                }

                try
                {
                    switch (action.ModificationType)
                    {
                        case ModType.Set:
                            if (targetToken.Type == JTokenType.String)
                            {
                                targetToken.Replace(action.ModificationValue);
                            }
                            else if (targetToken.Type == JTokenType.Float)
                            {
                                targetToken.Replace(Convert.ToDouble(action.ModificationValue));
                            }
                            break;
                        case ModType.Add:
                        case ModType.Subtract:
                        case ModType.Multiply:
                        case ModType.Divide:
                            double targetValue = targetToken.Value<double>();
                            double modificationValue = double.Parse(action.ModificationValue);

                            switch (action.ModificationType)
                            {
                                case ModType.Add:
                                    targetValue += modificationValue;
                                    break;
                                case ModType.Subtract:
                                    targetValue -= modificationValue;
                                    break;
                                case ModType.Multiply:
                                    targetValue *= modificationValue;
                                    break;
                                case ModType.Divide:
                                    targetValue /= modificationValue;
                                    break;
                            }

                            if (targetToken.Type == JTokenType.Float)
                            {
                                targetToken.Replace(Convert.ToDouble(targetValue));
                            }
                            else
                            {
                                targetToken.Replace(targetValue);
                            }
                            break;
                        case ModType.Expression:
                            var variables = new Dictionary<string, FloatingPoint>();

                            var start = action.ModificationValue.IndexOf('{');
                            var end = action.ModificationValue.IndexOf('}');
                            var propertyName = action.ModificationValue.Substring(start + 1, end - start - 1);

                            var valueRetrieval = GetValueFromDataInput(inputData, propertyName);
                            if (!valueRetrieval.IsSuccess) return Result<JObject>.Failure(valueRetrieval.Error);

                            var propertyValue = (double)valueRetrieval.Value;

                            var expressionValue = action.ModificationValue.Replace("{" + propertyName + "}", propertyName);

                            variables.Add(propertyName, propertyValue);

                            Expression e;

                            try
                            {
                                e = Infix.ParseOrThrow(expressionValue);
                            }
                            catch (Exception ex)
                            {
                                return Result<JObject>.Failure($"Invalid modification value: {ex.Message}");
                            }

                            double result = Evaluate.Evaluate(variables, e).RealValue;
                            targetToken.Replace(JToken.FromObject(result));

                            break;
                        case ModType.Append:
                        case ModType.Prepend:
                            if (targetToken.Type == JTokenType.String)
                            {
                                targetToken.Replace(
                                    action.ModificationType == ModType.Append
                                        ? ((string)targetToken + action.ModificationValue)
                                        : (action.ModificationValue + (string)targetToken));
                            }
                            else
                            {
                                return Result<JObject>.Failure($"Invalid operation {action.ModificationType} on non-string type.");
                            }
                            break;
                        default:
                            return Result<JObject>.Failure($"Invalid modification type {action.ModificationType}.");
                    }
                }
                catch (Exception ex)
                {
                    return Result<JObject>.Failure($"Error while modifying field {action.TargetProperty}: {ex.Message}");
                }
            }

            return Result<JObject>.Success(JObject.FromObject(inputData));
        }

        private JObject BuildOutputData(RuleDto rule, Dictionary<string, object> validatedData)
        {
            var outputData = new JObject();

            foreach (var property in rule.Properties.Where(rp => rp.Direction == "O" || rp.Direction == "B"))
            {
                RuleProperty currentProperty = _mapper.Map<RuleProperty>(property);

                if (currentProperty.Type == PropertyType.ObjectType && currentProperty.SubProperties.Count > 0)
                {
                    outputData[currentProperty.Name] = BuildNestedOutputData(currentProperty, validatedData);
                }
                else
                {
                    var value = validatedData.ContainsKey(currentProperty.Name)
                        ? validatedData[currentProperty.Name]
                        : GetDefaultValue(currentProperty.Type);
                    outputData[currentProperty.Name] = JToken.FromObject(value);
                }
            }

            return outputData;
        }

        private JObject BuildNestedOutputData(RuleProperty currentProperty, Dictionary<string, object> validatedData)
        {
            var outputData = new JObject();
            string propertyNamePrefix = currentProperty.Name + "_";

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
                    var value = validatedData.ContainsKey(fullPropertyName)
                        ? validatedData[fullPropertyName]
                        : GetDefaultValue(currentSubProperty.Type);
                    outputData[currentSubProperty.Name] = JToken.FromObject(value);
                }
            }

            return outputData;
        }

        private object GetDefaultValue(PropertyType type)
        {
            switch (type)
            {
                case PropertyType.StringType:
                    return string.Empty;
                case PropertyType.NumberType:
                    return 0.0;
                case PropertyType.BooleanType:
                    return false;
                case PropertyType.ObjectType:
                    return new JObject();
                default:
                    throw new ArgumentException($"Invalid property type: {type}");
            }
        }
    }
}
