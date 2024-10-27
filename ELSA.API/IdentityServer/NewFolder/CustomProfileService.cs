using IdentityModel;
using IdentityServer.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityServer.NewFolder
{
    public class CustomProfileService(UserManager<User> _userManager) : IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            var claims = new List<Claim>();

            // Lấy role của user và thêm vào claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }

            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            if (user != null)
            {
                // Kiểm tra xem tài khoản có bị khóa hay không
                var isLockedOut = await _userManager.IsLockedOutAsync(user);

                // Kiểm tra trạng thái "active" của user (custom)
                var isActive = !isLockedOut && user.IsActive;

                // Gán kết quả vào context
                context.IsActive = isActive;
            }
            else
            {
                // Nếu không tìm thấy user, thì không hoạt động
                context.IsActive = false;
            }
        }
    }
}
