using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    public class RuleExecutionPolicy : IAuthorizationRequirement
    {
    }

    public class RuleExecutionPolicyHandler : AuthorizationHandler<RuleExecutionPolicy>
    {
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RuleExecutionPolicyHandler(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RuleExecutionPolicy requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Task.CompletedTask;

            var idFromContext = _httpContextAccessor.HttpContext?.Request.RouteValues.SingleOrDefault(x => x.Key == "id").Value?.ToString();
            var parsedGuid = new Guid();
            var ruleProjectId = new Guid();

            if (idFromContext != null)
            {
                parsedGuid = Guid.Parse(idFromContext);
                if (_httpContextAccessor.HttpContext.Request.RouteValues.Any(x => x.Value.ToString() == "ExecuteTable"))
                {
                    ruleProjectId = _dbContext.DecisionTables.SingleOrDefault(t => t.Id == parsedGuid).RuleProjectId;
                }
                else if (_httpContextAccessor.HttpContext.Request.RouteValues.Any(x => x.Value.ToString() == "ExecuteRule"))
                {
                    ruleProjectId = _dbContext.Rules.SingleOrDefault(r => r.Id == parsedGuid).RuleProjectId;
                }
            }

            var member = _dbContext.RuleProjectMembers
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.AppUserId == userId && x.RuleProjectId == ruleProjectId)
                .Result;

            if (member == null) return Task.CompletedTask;

            if (member != null) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}