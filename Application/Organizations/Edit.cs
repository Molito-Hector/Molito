using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Organizations
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Organization Organization { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Organization).SetValidator(new OrganizationValidator());
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
                var organization = await _context.Organizations
                    .FirstOrDefaultAsync(o => o.Id == request.Organization.Id);

                if (organization == null) return null;

                organization.Name = request.Organization.Name;
                organization.Description = request.Organization.Description;

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to update organization");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}