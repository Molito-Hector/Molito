namespace Domain
{
    public class DecisionRow
    {
        public Guid Id { get; set; }
        public Guid TableId { get; set; }
        public DecisionTable DecisionTable { get; set; }
        public ICollection<ConditionValue> Values { get; set; }
        public ICollection<ActionValue> ActionValues { get; set; }
    }
}