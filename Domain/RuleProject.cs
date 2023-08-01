namespace Domain
{
    public class RuleProject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<RuleProperty> Properties { get; set; }
        public ICollection<Rule> StandardRules { get; set; }
        public ICollection<DecisionTable> DecisionTables { get; set; }
        public ICollection<RuleProjectMember> Members { get; set; } = new List<RuleProjectMember>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}