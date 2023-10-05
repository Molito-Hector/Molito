using Application.Core;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.DecisionTables
{
    public class EditActionColumn
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid TableId { get; set; }
            public Domain.Action Action { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Action).SetValidator(new ActionValidator());
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
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    var currentTable = await _context.DecisionTables
                    .Include(t => t.Rows).ThenInclude(r => r.Values)
                    .Include(t => t.Rows).ThenInclude(r => r.ActionValues)
                    .FirstOrDefaultAsync(t => t.Id == request.TableId);

                    var result = await _context.SaveChangesAsync() > 0;

                    if (!result) throw new Exception("Failed to modify action column");

                    await transaction.CommitAsync();

                    return Result<Unit>.Success(Unit.Value);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Result<Unit>.Failure(ex.Message);
                }
            }
        }
    }
}