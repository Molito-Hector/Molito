using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Organizations
{
    public class Details
    {
        public class Query : IRequest<Result<Organization>>
        {
            public Guid OrgId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Organization>>
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

            public async Task<Result<Organization>> Handle(Query request, CancellationToken cancellationToken)
            {
                var organization = await _context.Organizations
                    .ProjectTo<Organization>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
                    .SingleOrDefaultAsync(x => x.Id == request.OrgId);

                return Result<Organization>.Success(organization);
            }
        }
    }
}