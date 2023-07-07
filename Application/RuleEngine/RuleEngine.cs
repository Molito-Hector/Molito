using Application.Interfaces;
using Domain;

namespace Application.RuleEngine
{
    public class RuleEngine : IRuleEngine
    {
        public bool ExecuteRule(RuleDto rule, IDictionary<string, object> data)
        {
            if (rule == null)
            {
                return false;
            }

            foreach (var condition in rule.Conditions)
            {
                if (!ConditionIsMet(condition, data))
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

        private bool ConditionIsMet(ConditionDto condition, IDictionary<string, object> data)
        {
            // Check condition against data here
            // For simplicity, let's assume your conditions only check for equality
            if (data.TryGetValue(condition.Field, out var fieldValue))
            {
                return fieldValue.ToString() == condition.Value;
            }

            return false;
        }

        private void PerformAction(ActionDto action, IDictionary<string, object> data)
        {
            Console.WriteLine($"Performing action: {action.Name}");
        }
    }
}