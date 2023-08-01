namespace Domain
{
    public class RuleProjectMember
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public Guid RuleProjectId { get; set; }
        public RuleProject RuleProject { get; set; }
        public bool IsOwner { get; set; }
    }
}