using Application.RuleEngine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace API.Controllers
{
    [AllowAnonymous]
    public class RuleEngineController : BaseApiController
    {
        [HttpPost("{id}/execute")]
        public async Task<IActionResult> ExecuteRule(Guid id, [FromBody] JObject data)
        {
            return HandleResult(await Mediator.Send(new Execute.Command { Id = id, Data = data }));
        }
    }
}