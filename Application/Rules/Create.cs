using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Rules
{
    public class Create
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
                var rule = new Rule
                {
                    Id = request.Rule.Id,
                    Name = request.Rule.Name,
                };

                foreach (var property in request.Rule.Properties)
                {
                    AddPropertyToContext(property, rule.Id);
                }

                foreach (var condition in request.Rule.Conditions)
                {
                    AddConditionToContext(condition, rule.Id);
                }

                foreach (var action in request.Rule.Actions)
                {
                    action.RuleId = rule.Id;
                    _context.Actions.Add(action);
                }

                _context.Rules.Add(rule);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to create rule");

                return Result<Unit>.Success(Unit.Value);
            }

            private void AddConditionToContext(Condition condition, Guid ruleId)
            {
                condition.RuleId = ruleId;
                _context.Conditions.Add(condition);

                if (condition.SubConditions != null)
                {
                    foreach (var subCondition in condition.SubConditions)
                    {
                        AddConditionToContext(subCondition, ruleId);
                    }
                }
            }

            private void AddPropertyToContext(RuleProperty property, Guid ruleId)
            {
                property.RuleId = ruleId;
                _context.RuleProperties.Add(property);

                if (property.SubProperties != null)
                {
                    foreach (var subProperty in property.SubProperties)
                    {
                        subProperty.Direction = property.Direction;
                        AddPropertyToContext(subProperty, ruleId);
                    }
                }
            }
        }
    }
}