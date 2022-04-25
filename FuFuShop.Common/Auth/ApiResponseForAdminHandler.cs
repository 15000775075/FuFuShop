using FuFuShop.Model.ViewModels.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.Encodings.Web;

namespace FuFuShop.Common.Auth
{
    public class ApiResponseForAdminHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public ApiResponseForAdminHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            //Response.StatusCode = StatusCodes.Status401Unauthorized;
            //await Response.WriteAsync(JsonConvert.SerializeObject(new ApiResponse(StatusCode.CODE401)));

            var jm = new AdminUiCallBack();
            jm.code = 401;
            jm.data = 401;
            jm.msg = "很抱歉，您无权访问该接口，请确保已经登录!";
            await Response.WriteAsync(JsonConvert.SerializeObject(jm));
        }

        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            //Response.StatusCode = StatusCodes.Status403Forbidden;
            //await Response.WriteAsync(JsonConvert.SerializeObject(new ApiResponse(StatusCode.CODE403)));

            var jm = new AdminUiCallBack();
            jm.code = 403;
            jm.msg = "很抱歉，您的访问权限等级不够，联系管理员!!";
            await Response.WriteAsync(JsonConvert.SerializeObject(jm));

        }

    }
}
