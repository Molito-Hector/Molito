using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Rules
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Rule Rule { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Rule).SetValidator(new RuleValidator());
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
                var rule = await _context.Rules
                    .Include(r => r.Conditions)
                    .Include(r => r.Actions)
                    .FirstOrDefaultAsync(r => r.Id == request.Rule.Id);

                if (rule == null) return null;

                rule.Name = request.Rule.Name;

                _context.Conditions.RemoveRange(rule.Conditions);
                foreach (var condition in request.Rule.Conditions)
                {
                    condition.RuleId = rule.Id;
                    _context.Conditions.Add(condition);
                }

                _context.Actions.RemoveRange(rule.Actions);
                foreach (var action in request.Rule.Actions)
                {
                    action.RuleId = rule.Id;
                    _context.Actions.Add(action);
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to update rule");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}