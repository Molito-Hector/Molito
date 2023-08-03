using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.RuleProperties
{
    public class EditProperty
    {
        public class Command : IRequest<Result<Unit>>
        {
            public RuleProperty RuleProperty { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.RuleProperty).SetValidator(new RulePropertyValidator());
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
                var property = await _context.RuleProperties
                    .Include(p => p.SubProperties)
                    .FirstOrDefaultAsync(r => r.Id == request.RuleProperty.Id);

                if (property == null) return null;

                property.Name = request.RuleProperty.Name;
                property.Type = request.RuleProperty.Type;
                property.Direction = request.RuleProperty.Direction;

                if (property.SubProperties != null) _context.RuleProperties.RemoveRange(property.SubProperties);

                foreach (RuleProperty subProperty in request.RuleProperty.SubProperties)
                {
                    subProperty.Direction = property.Direction;
                    subProperty.ProjectId = property.ProjectId;
                    subProperty.ParentPropertyId = property.Id;
                    _context.RuleProperties.Add(subProperty);
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to update property");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}