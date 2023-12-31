namespace Domain
{
    public class Rule
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid RuleProjectId { get; set; }
        public RuleProject RuleProject { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Condition> Conditions { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}