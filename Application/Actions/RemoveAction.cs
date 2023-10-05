using Application.Core;
using MediatR;
using Persistence;

namespace Application.Actions
{
    public class RemoveAction
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
                var action = await _context.Actions.FindAsync(request.Id);

                if (action == null) return null;

                _context.Remove(action);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to delete the action");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}