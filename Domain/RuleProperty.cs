namespace Domain
{
    public enum PropertyType
    {
        StringType,
        NumberType,
        BooleanType,
        ObjectType
    }

    public class RuleProperty
    {
        public Guid Id { get; set; }
        public Guid RuleId { get; set; }
        public Rule Rule { get; set; }
        public string Name { get; set; }
        public PropertyType Type { get; set; }
        public char Direction { get; set; }
        public Guid? ParentPropertyId { get; set; }
        public RuleProperty ParentProperty { get; set; }
        public ICollection<RuleProperty> SubProperties { get; set; }
    }
}