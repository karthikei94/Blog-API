using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers.v2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[Controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {

        [HttpGet]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Get()
        {
            return Ok();
        }
    }
}