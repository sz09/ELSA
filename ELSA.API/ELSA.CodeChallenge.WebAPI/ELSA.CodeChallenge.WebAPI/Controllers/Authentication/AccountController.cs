using IdentityServer.Clients;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ELSA.WebAPI.Const;
using ELSA.WebAPI.Models.Authentication;
using ELSA.WebAPI.Models;

namespace ELSA.WebAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [ApiVersionNeutral]
    [Route("account")]
    [ApiExplorerSettings(GroupName = AuthConst.GROUP_AUTH)]
    public class AccountController(IIdentityServerClient identityServerClient) : ControllerBase
    {
        /// <summary>
        /// Login with provide registered account
        /// </summary>
        /// <param name="loginModel"><see cref="LoginModel"/></param>
        /// <returns></returns>
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel loginModel)
        {
            var accessToken = await identityServerClient.RequestPasswordTokenAsync(
                username: loginModel.Username,
                password: loginModel.Password,
                accessType: AccessType.AdminUserFullAccess,
                includeUserInfo: true,
                includeRefreshToken: loginModel.KeepMeLogIn
            );
            if (accessToken.IsError)
            {
                return BadRequest("Invalid username or password");
            }
            if (loginModel.KeepMeLogIn)
            {
                var refreshTokenResponse = await identityServerClient.GetRefreshTokenAsync(accessToken.RefreshToken);

                return Ok(new
                {
                    accessToken.AccessToken,
                    accessToken.ExpiresIn,
                    RefreshTokenIssuedIn = refreshTokenResponse.CreationTime,
                    RefreshTokenLifetime = refreshTokenResponse.Lifetime,
                    accessToken.RefreshToken
                });
            }

            return Ok(new
            {
                accessToken.AccessToken,
                accessToken.ExpiresIn,
                accessToken.RefreshToken
            });
        }
        /// <summary>
        /// Retrive new access token with valid refresh token
        /// </summary>
        /// <param name="model"><see cref="RefreshTokenModel"/></param>
        /// <returns></returns>
        [Route("refresh-token")]
        [HttpPost]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenModel model)
        {
            var result = await identityServerClient.RequestRefreshTokenAsync(refreshToken: model.Token);
            var refreshTokenResponse = await identityServerClient.GetRefreshTokenAsync(model.Token);
            return Ok(new
            {
                result.AccessToken,
                result.ExpiresIn,
                result.RefreshToken,
                RefreshTokenIssuedIn = refreshTokenResponse.CreationTime,
                RefreshTokenLifetime = refreshTokenResponse.Lifetime,
            });
        }

        /// <summary>
        /// Get user's allowance claims  
        /// </summary>
        /// <param name="model"><see cref="AccessTokenModel"/></param>
        /// <returns></returns>
        [Route("logout")]
        [HttpPost]
        public async Task<IActionResult> LogoutAsync()
        {
            var context = HttpContext;
            if (context.User?.IsAuthenticated() == true)
            {
                //TODO: context.User.Logout();
            }
            return Ok();
        }
    }
}

