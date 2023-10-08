namespace Domain
{
    public class RuleProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<RulePropertyDto> Properties { get; set; }
        public ICollection<RuleListDto> StandardRules { get; set; }
        public ICollection<DecisionTableListDto> DecisionTables { get; set; }
        public ICollection<RuleProjectMemberDto> Members { get; set; }
        public string Owner { get; set; }
    }

    public class RuleProjectListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<RuleProjectMemberDto> Members { get; set; }
    }

    public class RuleProjectMemberDto
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
    }

    public class RulePropertyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Direction { get; set; }
        public List<SubpropertyDto> SubProperties { get; set; }
    }

    public class SubpropertyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}