using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Organizations
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
                var organization = await _context.Organizations
                    .Include(a => a.Members).ThenInclude(u => u.AppUser)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (organization == null) return null;

                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);

                if (user == null) return null;

                var ownerUsername = organization.Members.FirstOrDefault(x => x.IsAdmin)?.AppUser?.UserName;

                if (ownerUsername == user.UserName) Result<Unit>.Failure("Admin user can't be removed from the Organization");

                var membership = organization.Members.FirstOrDefault(x => x.AppUser.UserName == user.UserName);

                if (membership != null && ownerUsername != user.UserName)
                    organization.Members.Remove(membership);

                if (membership == null)
                {
                    membership = new OrganizationMember
                    {
                        AppUser = user,
                        Organization = organization,
                        IsAdmin = false
                    };

                    organization.Members.Add(membership);
                }

                var result = await _context.SaveChangesAsync() > 0;

                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating membership");
            }
        }
    }
}