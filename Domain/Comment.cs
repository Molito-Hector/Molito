namespace Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public AppUser Author { get; set; }
        public Rule Rule { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}