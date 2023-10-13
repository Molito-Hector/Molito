using Application.Core;
using Application.RuleProjects;
using Application.RuleProperties;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class RuleProjectsController : BaseApiController
    {
        [Authorize]
        [HttpGet] //api/ruleprojects
        public async Task<IActionResult> GetRuleProjects([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpGet("{id}")] //api/ruleprojects/{id}
        public async Task<IActionResult> GetRuleProject(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRuleProject(RuleProject ruleProject)
        {
            return HandleResult(await Mediator.Send(new Create.Command { RuleProject = ruleProject }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditRuleProject(Guid id, RuleProject ruleProject)
        {
            ruleProject.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { RuleProject = ruleProject }));
        }

        [Authorize(Policy = "IsRuleProjectOwner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRuleProject(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpPost("{id}/addProperties")]
        public async Task<IActionResult> AddProperties(Guid id, [FromBody] ICollection<RuleProperty> ruleProperties)
        {
            return HandleResult(await Mediator.Send(new AddProperties.Command { ProjectId = id, RuleProperties = ruleProperties }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpDelete("{id}/removeProperty/{propId}")]
        public async Task<IActionResult> RemoveProperty(Guid propId)
        {
            return HandleResult(await Mediator.Send(new RemoveProperty.Command { Id = propId }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpPut("{id}/editProperty")]
        public async Task<IActionResult> EditProperty(RuleProperty property)
        {
            return HandleResult(await Mediator.Send(new EditProperty.Command { RuleProperty = property }));
        }

        [Authorize(Policy = "IsRuleProjectOwner")]
        [HttpPost("{id}/updateMember")]
        public async Task<IActionResult> UpdateMembership(Guid id, UpdateMembershipDto dto)
        {
            return HandleResult(await Mediator.Send(new UpdateMembership.Command { Id = id, UserName = dto.Username }));
        }
    }
}