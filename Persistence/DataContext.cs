using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<RuleProperty> RuleProperties { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Domain.Action> Actions { get; set; }
        public DbSet<ActivityAttendee> ActivityAttendees { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserFollowing> UserFollowings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ActivityAttendee>(x => x.HasKey(aa => new { aa.AppUserId, aa.ActivityId }));

            builder.Entity<ActivityAttendee>()
                .HasOne(u => u.AppUser)
                .WithMany(a => a.Activities)
                .HasForeignKey(aa => aa.AppUserId);

            builder.Entity<ActivityAttendee>()
                .HasOne(u => u.Activity)
                .WithMany(a => a.Attendees)
                .HasForeignKey(aa => aa.ActivityId);

            builder.Entity<Condition>()
                .HasOne(u => u.Rule)
                .WithMany(a => a.Conditions)
                .HasForeignKey(aa => aa.RuleId);

            builder.Entity<Condition>()
                .HasMany(c => c.SubConditions)
                .WithOne(c => c.ParentCondition)
                .HasForeignKey(c => c.ParentConditionId);

            builder.Entity<RuleProperty>()
                .HasOne(u => u.Rule)
                .WithMany(a => a.Properties)
                .HasForeignKey(aa => aa.RuleId);

            builder.Entity<RuleProperty>()
                .HasMany(c => c.SubProperties)
                .WithOne(c => c.ParentProperty)
                .HasForeignKey(c => c.ParentPropertyId);

            builder.Entity<Domain.Action>()
                .HasOne(u => u.Rule)
                .WithMany(a => a.Actions)
                .HasForeignKey(aa => aa.RuleId);

            builder.Entity<Comment>()
                .HasOne(a => a.Rule)
                .WithMany(c => c.Comments)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserFollowing>(b =>
            {
                b.HasKey(k => new { k.ObserverId, k.TargetId });

                b.HasOne(o => o.Observer)
                    .WithMany(f => f.Followings)
                    .HasForeignKey(o => o.ObserverId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(o => o.Target)
                    .WithMany(f => f.Followers)
                    .HasForeignKey(o => o.TargetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}