using Application.Core;
using Application.Organizations;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class OrganizationsController : BaseApiController
    {
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrg(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { OrgId = id }));
        }

        [Authorize(Roles = "Admin, OrgAdmin")]
        [HttpPost]
        public async Task<IActionResult> CreateOrg(Organization org)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Organization = org }));
        }

        [Authorize(Roles = "Admin, OrgAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrg(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        [Authorize(Roles = "Admin, OrgAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditOrg(Guid id, Organization organization)
        {
            organization.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { Organization = organization }));
        }

        [Authorize]
        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetOrgMembers([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new ListMembers.Query { Params = param }));
        }

        [Authorize(Roles = "Admin, OrgAdmin")]
        [HttpPost("{id}/updateMember")]
        public async Task<IActionResult> UpdateMembership(Guid id, UpdateMembershipDto dto)
        {
            return HandleResult(await Mediator.Send(new UpdateMembership.Command { Id = id, UserName = dto.Username }));
        }
    }
}