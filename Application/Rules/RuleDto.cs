namespace Domain
{
    public class RuleProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<RulePropertyDto> Properties { get; set; }
        public ICollection<RuleDto> StandardRules { get; set; }
        public ICollection<DecisionTableDto> DecisionTables { get; set; }
        public ICollection<RuleProjectMemberDto> Members { get; set; }
    }

    public class RuleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ConditionDto> Conditions { get; set; }
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

    public class DecisionTableDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EvalType EvaluationType { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<DecisionRowDto> Rows { get; set; }
    }

    public class DTWithProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EvalType EvaluationType { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<DecisionRowDto> Rows { get; set; }
        public RuleProjectDto RuleProject { get; set; }
    }

    public class DecisionRowDto
    {
        public Guid Id { get; set; }
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
        public ICollection<ActionDto> Actions { get; set; }
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
        public string TargetProperty { get; set; }
        public ModType? ModificationType { get; set; }
        public string ModificationValue { get; set; }
    }

    public class RuleProjectMemberDto
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
    }
}