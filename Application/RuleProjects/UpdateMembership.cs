using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.RuleProjects
{
    public class UpdateMembership
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
            public string UserName { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var ruleProject = await _context.RuleProjects
                    .Include(a => a.Members).ThenInclude(u => u.AppUser)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (ruleProject == null) return null;

                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);

                if (user == null) return null;

                var ownerUsername = ruleProject.Members.FirstOrDefault(x => x.IsOwner)?.AppUser?.UserName;

                if (ownerUsername == user.UserName) return Result<Unit>.Failure("Owner can't be removed from the Rule Project");

                var membership = ruleProject.Members.FirstOrDefault(x => x.AppUser.UserName == user.UserName);

                if (membership != null && ownerUsername != user.UserName)
                    ruleProject.Members.Remove(membership);

                if (membership == null)
                {
                    membership = new RuleProjectMember
                    {
                        AppUser = user,
                        RuleProject = ruleProject,
                        IsOwner = false
                    };

                    ruleProject.Members.Add(membership);
                }

                var result = await _context.SaveChangesAsync() > 0;

                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating membership");
            }
        }
    }
}