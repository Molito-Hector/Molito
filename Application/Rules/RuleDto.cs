namespace Domain
{
    public class RuleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<RulePropertyDto> Properties { get; set; }
        public ICollection<ConditionDto> Conditions { get; set; }
        public ICollection<ActionDto> Actions { get; set; }
    }

    public class RulePropertyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Direction { get; set; }
        public List<SubpropertyDto> SubProperties { get; set; }
    }

    public class SubpropertyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class ConditionDto
    {
        public Guid Id { get; set; }
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string LogicalOperator { get; set; }
        public List<SubConditionDto> SubConditions { get; set; }
    }

    public class SubConditionDto
    {
        public Guid Id { get; set; }
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }

    public class ActionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}