namespace Domain
{
    public class Rule
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<RuleProperty> Properties { get; set; }
        public ICollection<Condition> Conditions { get; set; }
        public ICollection<Action> Actions { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}