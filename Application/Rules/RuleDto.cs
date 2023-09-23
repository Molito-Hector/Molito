namespace Domain
{
    public class RuleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ConditionDto> Conditions { get; set; }
    }

    public class RuleListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RuleWithProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ConditionDto> Conditions { get; set; }
        public RuleProjectDto RuleProject { get; set; }
    }

    public class ConditionDto
    {
        public Guid Id { get; set; }
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string LogicalOperator { get; set; }
        public ICollection<ActionDto> Actions { get; set; }
        public List<ConditionDto> SubConditions { get; set; }
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
        public Guid? RowId { get; set; }
        public string TargetProperty { get; set; }
        public string ModificationType { get; set; }
        public string ModificationValue { get; set; }
    }
}