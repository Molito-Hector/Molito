namespace Domain
{
    public class Condition
    {
        public Guid Id { get; set; }
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string LogicalOperator { get; set; }
        public Guid? ParentConditionId { get; set; }
        public Condition ParentCondition { get; set; }
        public ICollection<Condition> SubConditions { get; set; }
        public ICollection<Action> Actions { get; set; }
        public Guid? RuleId { get; set; }
        public Rule Rule { get; set; }
        public Guid? TableId { get; set; }
        public int TableColumnIndex { get; set; }
        public DecisionTable DecisionTable { get; set; }
    }
}