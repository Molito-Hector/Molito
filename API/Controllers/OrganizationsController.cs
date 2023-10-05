using Application.Core;
using Application.Organizations;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class OrganizationsController : BaseApiController
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrg(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { OrgId = id }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrg(Organization org)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Organization = org }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrg(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditOrg(Guid id, Organization organization)
        {
            organization.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { Organization = organization }));
        }

        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetOrgMembers([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new ListMembers.Query { Params = param }));
        }

        [HttpPost("{id}/updateMember")]
        public async Task<IActionResult> UpdateMembership(Guid id, string username)
        {
            return HandleResult(await Mediator.Send(new UpdateMembership.Command { Id = id, UserName = username }));
        }
    }
}