using Application.Conditions;
using Application.Core;
using Application.Rules;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class RulesController : BaseApiController
    {
        [Authorize]
        [HttpGet] //api/rules
        public async Task<IActionResult> GetRules([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpGet("{id}")] //api/rules/{id}
        public async Task<IActionResult> GetRule(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpPost]
        public async Task<IActionResult> CreateRule(Rule rule)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Rule = rule }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditRule(Guid id, Rule rule)
        {
            rule.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { Rule = rule }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRule(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpPost("{id}/addConditions")]
        public async Task<IActionResult> AddConditions(Guid id, [FromBody] ICollection<Condition> conditions)
        {
            return HandleResult(await Mediator.Send(new AddConditions.Command { RuleId = id, Conditions = conditions }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpDelete("removeCondition/{condId}")]
        public async Task<IActionResult> RemoveProperty(Guid condId)
        {
            return HandleResult(await Mediator.Send(new RemoveCondition.Command { Id = condId }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpPut("editCondition")]
        public async Task<IActionResult> RemoveProperty(Condition condition)
        {
            return HandleResult(await Mediator.Send(new EditCondition.Command { Condition = condition }));
        }
    }
}