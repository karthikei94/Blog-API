using System.Security.Claims;
using System.Text.Json.Serialization;
using Asp.Versioning;
using BlogApp.Services;
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
        private readonly IMemoryCachingService _memoryCachingService;

        public SampleController(IMemoryCachingService memoryCachingService) => _memoryCachingService = memoryCachingService;

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

        [HttpPost("TrialCache")]
        [MapToApiVersion("2.0")]
        [AllowAnonymous]
        public async Task<IActionResult> TrialCache()
        {

            var result = await _memoryCachingService.GetOrCreateAsync<List<string>>("Users", async () =>
            {
                var result = await GetRandom();
                // await Task.Delay(1000);
                return result;
            });
            return Ok(result);
        }
        
        [NonAction]
        public async Task<List<string>> GetRandom()
        {
            Random rd = new Random();
            var list = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                await Task.Delay(100);
                var trail = rd.Next(1, 98);
                list.Add(trail.ToString());
            }
            return list;
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