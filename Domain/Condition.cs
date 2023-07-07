namespace Domain
{
    public class Condition
    {
        public Guid Id { get; set; }
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public Guid RuleId { get; set; }
        public Rule Rule { get; set; }
    }
}