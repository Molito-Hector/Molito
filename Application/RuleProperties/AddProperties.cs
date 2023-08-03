using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.RuleProperties
{
    public class AddProperties
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid ProjectId { get; set; }
            public ICollection<RuleProperty> RuleProperties { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleForEach(x => x.RuleProperties).SetValidator(new RulePropertyValidator());
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
                foreach (RuleProperty property in request.RuleProperties)
                {
                    AddPropertyToContext(property, request.ProjectId);
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to create rule property");

                return Result<Unit>.Success(Unit.Value);
            }

            private void AddPropertyToContext(RuleProperty property, Guid projectId)
            {
                property.ProjectId = projectId;
                _context.RuleProperties.Add(property);

                if (property.SubProperties != null)
                {
                    foreach (var subProperty in property.SubProperties)
                    {
                        subProperty.Direction = property.Direction;
                        AddPropertyToContext(subProperty, projectId);
                    }
                }
            }
        }
    }
}