namespace Domain
{
    public class OrganizationMember
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public bool IsAdmin { get; set; }
    }
}