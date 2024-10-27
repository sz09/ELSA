using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using ELSA.IdentityServer.Models;

namespace ELSA.IdentityServer.Controllers
{
    [Route("token")]
    [ApiController]
    public class TokenController : BaseController
    {
        private readonly IRefreshTokenStore _refreshTokenStore;
        public TokenController(IRefreshTokenStore refreshTokenStore)
        {
            _refreshTokenStore = refreshTokenStore;
        }

        [HttpPost("retrieve")]
        public async Task<IActionResult> GetRefreshTokenAsync([FromBody] RefreshTokenWrapper model)
        {
            var tokenResponse = await _refreshTokenStore.GetRefreshTokenAsync(model.RefreshToken);
            if(tokenResponse == null){
                return null;
            }
            
            return Ok(new 
            {
                tokenResponse.ConsumedTime,
                tokenResponse.CreationTime,
                tokenResponse.Lifetime
            });
        }
    }
}
