using Application.Core;
using Application.DecisionTables;
using Domain;
using FluentValidation;
using MathNet.Symbolics;
using MediatR;
using Persistence;

namespace Application.Actions
{
    public class AddActions
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid RuleId { get; set; }
            public string Predicate { get; set; }
            public ICollection<Domain.Action> Actions { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleForEach(x => x.Actions).SetValidator(new ActionValidator());
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
                foreach (Domain.Action action in request.Actions)
                {
                    AddActionToContext(action, request.RuleId, request.Predicate);
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to create actions");

                return Result<Unit>.Success(Unit.Value);
            }

            private void AddActionToContext(Domain.Action action, Guid ruleId, string requestPredicate)
            {
                if (requestPredicate == "Table")
                {
                    action.TableId = ruleId;
                }

                _context.Actions.Add(action);

                AddDefaultValueForNewAction(action);
            }

            private void AddDefaultValueForNewAction(Domain.Action action)
            {
                var rows = _context.DecisionRows.Where(r => r.TableId == action.TableId).ToList();

                foreach (var row in rows)
                {
                    row.ActionValues ??= new List<ActionValue>();

                    row.ActionValues.Add(new ActionValue
                    {
                        ActionId = action.Id,
                        DecisionRowId = row.Id,
                        Value = ""
                    });
                }
            }
        }
    }
}