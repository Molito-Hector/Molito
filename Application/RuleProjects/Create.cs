using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.RuleProjects
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
        {
            public RuleProject RuleProject { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.RuleProject).SetValidator(new RuleProjectValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            public IUserAccessor _userAccessor { get; set; }
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                var owner = new RuleProjectMember
                {
                    AppUser = user,
                    RuleProject = request.RuleProject,
                    IsOwner = true
                };

                request.RuleProject.Members.Add(owner);

                _context.RuleProjects.Add(request.RuleProject);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to create rule project");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}