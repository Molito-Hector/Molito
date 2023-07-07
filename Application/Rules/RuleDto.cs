namespace Domain
{
    public class RuleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<ConditionDto> Conditions { get; set; }
        public ICollection<ActionDto> Actions { get; set; }
    }

    public class ConditionDto
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