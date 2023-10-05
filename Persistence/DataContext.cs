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
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<RuleProject> RuleProjects { get; set; }
        public DbSet<RuleProperty> RuleProperties { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<DecisionTable> DecisionTables { get; set; }
        public DbSet<DecisionRow> DecisionRows { get; set; }
        public DbSet<ConditionValue> ConditionValues { get; set; }
        public DbSet<ActionValue> ActionValues { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Domain.Action> Actions { get; set; }
        public DbSet<RuleProjectMember> RuleProjectMembers { get; set; }
        public DbSet<OrganizationMember> OrganizationMembers { get; set; }
        public DbSet<ActivityAttendee> ActivityAttendees { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserFollowing> UserFollowings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ActivityAttendee>(x => x.HasKey(aa => new { aa.AppUserId, aa.ActivityId }));
            builder.Entity<RuleProjectMember>(x => x.HasKey(aa => new { aa.AppUserId, aa.RuleProjectId }));
            builder.Entity<OrganizationMember>(x => x.HasKey(aa => new { aa.AppUserId, aa.OrganizationId }));

            builder.Entity<ActivityAttendee>()
                .HasOne(u => u.AppUser)
                .WithMany(a => a.Activities)
                .HasForeignKey(aa => aa.AppUserId);

            builder.Entity<ActivityAttendee>()
                .HasOne(u => u.Activity)
                .WithMany(a => a.Attendees)
                .HasForeignKey(aa => aa.ActivityId);

            builder.Entity<RuleProjectMember>()
                .HasOne(u => u.AppUser)
                .WithMany(a => a.RuleProjects)
                .HasForeignKey(aa => aa.AppUserId);

            builder.Entity<RuleProjectMember>()
                .HasOne(u => u.RuleProject)
                .WithMany(a => a.Members)
                .HasForeignKey(aa => aa.RuleProjectId);

            builder.Entity<OrganizationMember>()
                .HasOne(u => u.Organization)
                .WithMany(a => a.Members)
                .HasForeignKey(aa => aa.OrganizationId);

            builder.Entity<Rule>()
                .HasOne(u => u.RuleProject)
                .WithMany(a => a.StandardRules)
                .HasForeignKey(aa => aa.RuleProjectId);

            builder.Entity<DecisionTable>()
                .HasOne(u => u.RuleProject)
                .WithMany(a => a.DecisionTables)
                .HasForeignKey(aa => aa.RuleProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DecisionRow>()
                .HasOne(u => u.DecisionTable)
                .WithMany(a => a.Rows)
                .HasForeignKey(aa => aa.TableId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ConditionValue>()
                .HasOne(u => u.DecisionRow)
                .WithMany(a => a.Values)
                .HasForeignKey(aa => aa.DecisionRowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ActionValue>()
                .HasOne(u => u.DecisionRow)
                .WithMany(a => a.ActionValues)
                .HasForeignKey(aa => aa.DecisionRowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Condition>()
                .HasOne(u => u.Rule)
                .WithMany(a => a.Conditions)
                .HasForeignKey(aa => aa.RuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Condition>()
                .HasOne(u => u.DecisionTable)
                .WithMany(a => a.Conditions)
                .HasForeignKey(aa => aa.TableId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Condition>()
                .HasMany(c => c.SubConditions)
                .WithOne(c => c.ParentCondition)
                .HasForeignKey(c => c.ParentConditionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RuleProperty>()
                .HasOne(u => u.RuleProject)
                .WithMany(a => a.Properties)
                .HasForeignKey(aa => aa.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RuleProperty>()
                .HasMany(c => c.SubProperties)
                .WithOne(c => c.ParentProperty)
                .HasForeignKey(c => c.ParentPropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Domain.Action>()
                .HasOne(u => u.Condition)
                .WithMany(a => a.Actions)
                .HasForeignKey(aa => aa.ConditionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Domain.Action>()
                .HasOne(u => u.DecisionTable)
                .WithMany(a => a.Actions)
                .HasForeignKey(aa => aa.TableId)
                .OnDelete(DeleteBehavior.Cascade);

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