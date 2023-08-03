using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
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
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var project = new RuleProject
                {
                    Id = request.RuleProject.Id,
                    Name = request.RuleProject.Name,
                    Description = request.RuleProject.Description
                };

                _context.RuleProjects.Add(project);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to create rule project");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}