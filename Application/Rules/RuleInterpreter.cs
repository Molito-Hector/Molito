using Domain;

namespace Application.Rules
{
    public class RuleInterpreter
    {
        public bool ApplyRule(object obj, Rule rule)
        {
            foreach (var condition in rule.Conditions)
            {
                var property = obj.GetType().GetProperty(condition.Field);
                var value = property.GetValue(obj);
                if (!Compare(value.ToString(), condition.Operator, condition.Value))
                {
                    return false;
                }
            }

            return true;
        }

        private bool Compare(string value1, string condOperator, string value2)
        {
            switch (condOperator)
            {
                case "==": return value1 == value2;
                case "!=": return value1 != value2;
                // add more cases here as needed
                default: throw new ArgumentException($"Invalid operator: {condOperator}");
            }
        }
    }
}