namespace Domain
{
    public class DecisionRow
    {
        public Guid Id { get; set; }
        public Guid TableId { get; set; }
        public DecisionTable DecisionTable { get; set; }
        public ICollection<Condition> Conditions { get; set; }
        public ICollection<Action> Actions { get; set; }
    }
}