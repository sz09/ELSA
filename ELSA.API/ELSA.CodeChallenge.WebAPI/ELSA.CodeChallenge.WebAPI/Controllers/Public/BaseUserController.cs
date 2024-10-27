using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ELSA.WebAPI.Const;

namespace ELSA.WebAPI.Controllers.Public
{
    [ApiExplorerSettings(GroupName = AuthConst.GROUP_USER)]
    [ApiController]
    [ApiVersion("1")]
    [AllowAnonymous]
    public abstract class BaseUserController : BaseController { }
}
