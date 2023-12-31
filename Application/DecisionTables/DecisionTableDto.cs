namespace Domain
{
    public class DecisionTableDto
    {
        public Guid Id { get; set; }
        public Guid RuleProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EvalType EvaluationType { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ConditionDto> Conditions { get; set; }
        public ICollection<ActionDto> Actions { get; set; }
        public ICollection<DecisionRowDto> Rows { get; set; }
    }

    public class DecisionTableListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DTWithProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EvalType EvaluationType { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ConditionDto> Conditions { get; set; }
        public ICollection<ActionDto> Actions { get; set; }
        public ICollection<DecisionRowDto> Rows { get; set; }
        public RuleProjectDto RuleProject { get; set; }
    }

    public class DecisionRowDto
    {
        public Guid Id { get; set; }
        public Guid TableId { get; set; }
        public ICollection<ConditionValueDto> Values { get; set; }
        public ICollection<ActionValueDto> ActionValues { get; set; }
    }

    public class ConditionValueDto
    {
        public Guid Id { get; set; }
        public Guid ConditionId { get; set; }
        public Guid DecisionRowId { get; set; }
        public string Value { get; set; }
    }

    public class ActionValueDto
    {
        public Guid Id { get; set; }
        public Guid ActionId { get; set; }
        public Guid DecisionRowId { get; set; }
        public string Value { get; set; }
    }
}