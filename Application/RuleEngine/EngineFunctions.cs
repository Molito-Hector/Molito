using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.RuleEngine
{
    public class EngineFunctions : IEngineFunctions
    {
        private readonly IMapper _mapper;
        public EngineFunctions(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Result<Dictionary<string, object>> ValidateInputData(RuleWithProjectDto rule, JObject inputData)
        {
            var validatedData = new Dictionary<string, object>();

            foreach (var inputProperty in rule.RuleProject.Properties.Where(x => x.Direction == "I" || x.Direction == "B"))
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

        public Result<object> GetValueFromDataInput(Dictionary<string, object> dataInput, string field)
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

        public JObject BuildOutputData(RuleWithProjectDto rule, Dictionary<string, object> validatedData)
        {
            var outputData = new JObject();

            foreach (var property in rule.RuleProject.Properties.Where(rp => rp.Direction == "O" || rp.Direction == "B"))
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