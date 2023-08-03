using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.RuleProjects
{
    public class Details
    {
        public class Query : IRequest<Result<RuleProjectDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<RuleProjectDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<RuleProjectDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var ruleProject = await _context.RuleProjects
                    .ProjectTo<RuleProjectDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (ruleProject == null) return null;

                return Result<RuleProjectDto>.Success(ruleProject);
            }
        }
    }
}