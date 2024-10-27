using Microsoft.AspNetCore.Mvc;
using ELSA.IdentityServer.Seed;

namespace ELSA.IdentityServer.Controllers
{
    [Route("temp")]
	public class TempDataController: BaseController
	{
		private readonly ISeeder _seeder;
        public TempDataController(ISeeder seeder)
        {
            _seeder = seeder;
        }

        [Route("seed")]
        public async Task<IActionResult> SeedDataAync()
		{
            await _seeder.SeedAsync();
            return Ok();
		}
	}
}

