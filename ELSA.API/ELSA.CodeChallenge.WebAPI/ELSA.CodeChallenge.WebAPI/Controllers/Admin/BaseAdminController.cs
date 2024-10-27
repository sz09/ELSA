using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ELSA.WebAPI.Const;

namespace ELSA.WebAPI.Controllers.Admin
{
    [Authorize(AuthConst.GROUP_ADMIN)]
    [ApiExplorerSettings(GroupName = AuthConst.GROUP_ADMIN)]
    [ApiVersion("1")]
    [ApiController]
    [ResponseCache(NoStore = true)]
    public abstract class BaseAdminController : BaseController { }
}
