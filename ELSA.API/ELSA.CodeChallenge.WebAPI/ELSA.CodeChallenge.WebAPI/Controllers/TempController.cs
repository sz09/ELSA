using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ELSA.Services.Interface;

namespace ELSA.WebAPI.Controllers
{
    [Route("temp")]
    [AllowAnonymous]
    [ApiController]
    [ApiVersionNeutral]
    [ApiExplorerSettings(GroupName = "Authenticate")]
    public class TempController(ITempService tempService) : Controller
    {
        [Route("seed")]
        [HttpPost]
        public async Task<IActionResult> SeedAsync([FromBody] SeedModel model)
        {
            await tempService.SeedAsync(model.Username, model.Email, model.Password);
            return Ok();
        }
    }


    public class SeedModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
