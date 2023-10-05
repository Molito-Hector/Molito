namespace Domain
{
    public class ActionValue
    {
        public Guid Id { get; set; }
        public Guid ActionId { get; set; }
        public Guid DecisionRowId { get; set; }
        public DecisionRow DecisionRow { get; set; }
        public string Value { get; set; }
    }
}