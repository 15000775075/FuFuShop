using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace FuFuShop.Common.Auth
{
    /// <summary>
    /// 权限授权处理器
    /// </summary>
    public class PermissionForAdminHandler : AuthorizationHandler<PermissionRequirement>
    {
        /// <summary>
        /// 验证方案提供对象
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }
        private readonly IHttpContextAccessor _accessor;


        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="schemes"></param>
        /// <param name="navigationRepository"></param>
        /// <param name="accessor"></param>
        public PermissionForAdminHandler(IAuthenticationSchemeProvider schemes
            , IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            Schemes = schemes;
        }


        // 重写异步处理程序
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {

            var httpContext = _accessor.HttpContext;

            context.Succeed(requirement);
        }
    }

}
