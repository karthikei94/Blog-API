using System.Security.Claims;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using Newtonsoft.Json;

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

        [HttpPost]
        [MapToApiVersion("2.0")]
        [Route("AuthFirebaseToken")]
        public IActionResult AuthFireBaseToken()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            dynamic userId = TokenValidate(claimsIdentity);
            return Ok(userId);
            var user = HttpContext.User;
            return Ok();
        }

        [HttpPost]
        [MapToApiVersion("2.0")]
        [Route("FirebaseEmail2Register")]
        public IActionResult FirebaseEmail2Register()
        {
            var user = HttpContext.User;
            var claims = ((ClaimsIdentity)user.Identity).Claims;
            var items = claims.ToList();
            var jsonStr = items.Last().Value;

            FirebaseIdentity fb = (FirebaseIdentity)System.Text.Json.JsonSerializer.Deserialize(jsonStr, typeof(FirebaseIdentity));
            string email = fb.Identities.Email[0];
            return Ok(email);
        }

        [NonAction]
        public string TokenValidate(ClaimsIdentity claimsIdentity)
        {
            List<Claim> claims = claimsIdentity.Claims.ToList();
            string userId = claims.FirstOrDefault(x => x.Type == "user_id").Value;

            return userId;
        }
    }
    [Serializable, JsonObject]
    public class FirebaseIdentity
    {
        [JsonProperty("Identities")]
        public Identities Identities { get; set; }
    }

    [Serializable, JsonObject]
    public class Identities
    {
        [JsonProperty("email")]
        public List<string> Email { get; set; }
    }
}