namespace Domain
{
    public class ConditionValue
    {
        public Guid Id { get; set; }
        public Guid ConditionId { get; set; }
        public Guid DecisionRowId { get; set; }
        public DecisionRow DecisionRow { get; set; }
        public string Value { get; set; }
    }
}