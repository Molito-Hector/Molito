using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Organizations
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Organization Organization { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Organization).SetValidator(new OrganizationValidator());
            }
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
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                var orgMember = new OrganizationMember
                {
                    AppUser = user,
                    Organization = request.Organization,
                    IsAdmin = true
                };

                request.Organization.Members.Add(orgMember);

                _context.Organizations.Add(request.Organization);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to create organization");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}