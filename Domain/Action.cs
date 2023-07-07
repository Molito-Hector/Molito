namespace Domain
{
    public class Action
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid RuleId { get; set; }
        public Rule Rule { get; set; }
    }
}