using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    public class IsRuleProjectOwner : IAuthorizationRequirement
    {
    }

    public class IsRuleProjectOwnerHandler : AuthorizationHandler<IsRuleProjectOwner>
    {
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IsRuleProjectOwnerHandler(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsRuleProjectOwner requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Task.CompletedTask;

            var ruleProjectId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues.SingleOrDefault(x => x.Key == "id").Value?.ToString());

            var member = _dbContext.RuleProjectMembers
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.AppUserId == userId && x.RuleProjectId == ruleProjectId)
                .Result;

            if (member == null) return Task.CompletedTask;

            if (member.IsOwner) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}