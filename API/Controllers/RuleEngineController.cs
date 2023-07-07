using Application.RuleEngine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    public class RuleEngineController : BaseApiController
    {
        [HttpPost("{id}/execute")]
        public async Task<IActionResult> ExecuteRule(Guid id, [FromBody] Dictionary<string, object> data)
        {
            return HandleResult(await Mediator.Send(new Execute.Command { Id = id, Data = data }));
        }
    }
}