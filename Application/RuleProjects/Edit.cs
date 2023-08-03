using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.RuleProjects
{
    public class Edit
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
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var ruleProject = await _context.RuleProjects
                    .FirstOrDefaultAsync(r => r.Id == request.RuleProject.Id);

                if (ruleProject == null) return null;

                ruleProject.Name = request.RuleProject.Name;
                ruleProject.Description = request.RuleProject.Description;

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to update rule project");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}