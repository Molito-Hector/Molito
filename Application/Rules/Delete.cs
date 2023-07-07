using Application.Core;
using MediatR;
using Persistence;

namespace Application.Rules
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
                var rule = await _context.Rules.FindAsync(request.Id);

                if (rule == null) return null;

                _context.Remove(rule);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to delete the rule");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}