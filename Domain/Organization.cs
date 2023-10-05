namespace Domain
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<OrganizationMember> Members { get; set; } = new List<OrganizationMember>();
    }
}