using Application.Core;
using MediatR;
using Persistence;

namespace Application.RuleProjects
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
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
                var ruleProject = await _context.RuleProjects.FindAsync(request.Id);

                if (ruleProject == null) return null;

                _context.Remove(ruleProject);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to delete the rule project");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}