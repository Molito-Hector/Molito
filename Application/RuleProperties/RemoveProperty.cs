using Application.Core;
using MediatR;
using Persistence;

namespace Application.RuleProperties
{
    public class RemoveProperty
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
                var property = await _context.RuleProperties.FindAsync(request.Id);

                if (property == null) return null;

                _context.Remove(property);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to delete the property");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}