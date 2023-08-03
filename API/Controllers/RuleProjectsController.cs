using Application.Core;
using Application.RuleProjects;
using Application.RuleProperties;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    public class RuleProjectsController : BaseApiController
    {
        [HttpGet] //api/ruleprojects
        public async Task<IActionResult> GetRuleProjects([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }

        [HttpGet("{id}")] //api/ruleprojects/{id}
        public async Task<IActionResult> GetRuleProject(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRuleProject(RuleProject ruleProject)
        {
            return HandleResult(await Mediator.Send(new Create.Command { RuleProject = ruleProject }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditRuleProject(Guid id, RuleProject ruleProject)
        {
            ruleProject.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { RuleProject = ruleProject }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRuleProject(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        [HttpPost("{id}/addProperties")]
        public async Task<IActionResult> AddProperties(Guid id, [FromBody] ICollection<RuleProperty> ruleProperties)
        {
            return HandleResult(await Mediator.Send(new AddProperties.Command { ProjectId = id, RuleProperties = ruleProperties }));
        }

        [HttpDelete("removeProperty/{propId}")]
        public async Task<IActionResult> RemoveProperty(Guid propId)
        {
            return HandleResult(await Mediator.Send(new RemoveProperty.Command { Id = propId }));
        }

        [HttpPut("editProperty")]
        public async Task<IActionResult> RemoveProperty(RuleProperty property)
        {
            return HandleResult(await Mediator.Send(new EditProperty.Command { RuleProperty = property }));
        }
    }
}