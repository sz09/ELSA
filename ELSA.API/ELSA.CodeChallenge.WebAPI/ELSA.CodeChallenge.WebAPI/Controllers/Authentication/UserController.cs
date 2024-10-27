using AutoMapper;
using IdentityServer.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ELSA.Services.Interface;
using ELSA.WebAPI.Models.Authentication;
using ELSA.Services.Models.Users;
using ELSA.WebAPI.Const;

namespace ELSA.WebAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [ApiVersionNeutral]
    [Route("users")]
    [ApiExplorerSettings(GroupName = AuthConst.GROUP_AUTH)]
    public class UserController(IIdentityServerClient identityServerClient, IUserService userService, IMapper mapper) : ControllerBase
    {
        /// <summary>
        /// Get user's allowance claims  
        /// </summary>
        /// <param name="model"><see cref="AccessTokenModel"/></param>
        /// <returns></returns>
        [Route("user-info")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GetUserInfoAsync()
        {
            var loggedInUserService = HttpContext.RequestServices.GetRequiredService<ILoggedInUserService>();
            var result = await identityServerClient.GetUserInfoAsync(accessToken: loggedInUserService.AccessToken);
            return Ok(result);
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> RegisterUserAsync([FromBody] Models.RegisterUserModel model)
        {
            await userService.RegisterUserAsync(mapper.Map<RegisterUserModel>(model));

            return Ok();
        }

        [Route("register-user-anonymous")]
        [HttpPost]
        public async Task<IActionResult> RegisterUserAnonymousAsync([FromBody] Models.RegisterUserAnonymousModel model)
        {
            return Ok(await userService.RegisterUserAnonymousAsync(model.Email, model.Username));
        }
    }
}

