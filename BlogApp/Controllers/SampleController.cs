using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers.v2
{
    [ApiVersion("2.0")]
    [Route("api/v2.0/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {

        [MapToApiVersion("2.0")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Get()
        {
            return Ok();
        }
    }
}