namespace Domain
{
    public enum ModType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Append,
        Prepend,
        Set,
        Expression
    }
    public class Action
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TargetProperty { get; set; }
        public ModType? ModificationType { get; set; }
        public string ModificationValue { get; set; }
        public Guid RuleId { get; set; }
        public Rule Rule { get; set; }
    }
}