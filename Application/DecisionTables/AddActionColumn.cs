using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.DecisionTables
{
    public class AddActionColumn
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
                    .FirstOrDefaultAsync(t => t.Id == request.TableId);

                    _context.Actions.Add(request.Action);

                    foreach (var row in currentTable.Rows)
                    {
                        var actionValue = new ActionValue
                        {
                            ActionId = request.Action.Id,
                            DecisionRowId = row.Id,
                            Value = ""
                        };

                        row.ActionValues.Add(actionValue);
                    }

                    var result = await _context.SaveChangesAsync() > 0;

                    if (!result) throw new Exception("Failed to add action to decision table");

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