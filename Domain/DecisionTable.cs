namespace Domain
{
    public enum EvalType
    {
        FirstHit,
        MultiHit
    }
    public class DecisionTable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid RuleProjectId { get; set; }
        public RuleProject RuleProject { get; set; }
        public EvalType EvaluationType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<RuleProperty> Properties { get; set; }
        public ICollection<DecisionRow> Rows { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}