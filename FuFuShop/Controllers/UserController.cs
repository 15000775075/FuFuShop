using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Auth;
using FuFuShop.Model.Entitys;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using FuFuShop.Services.User;
using FuFuShop.Services.WeChat;
using FuFuShop.WeChat.Enums;
using FuFuShop.WeChat.Services.HttpClients;
using FuFuShop.WeChat.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nito.AsyncEx;
using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FuFuShop.Controllers
{
    /// <summary>
    /// 用户操作事件
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AsyncLock _mutex = new AsyncLock();
        private readonly IUserWeChatInfoServices _userWeChatInfoServices;
        private readonly IUserServices _userServices;
        private readonly IWeChatApiHttpClientFactory _weChatApiHttpClientFactory;
        private readonly PermissionRequirement _permissionRequirement;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserLogServices _userLogServices;

        public UserController(
            IUserServices userServices,
            IUserWeChatInfoServices userWeChatInfoServices,
            IWeChatApiHttpClientFactory weChatApiHttpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IUserLogServices userLogServices,
            PermissionRequirement permissionRequirement)
        {
            _userLogServices = userLogServices;
            _userServices = userServices;
            _userWeChatInfoServices = userWeChatInfoServices;
            _weChatApiHttpClientFactory = weChatApiHttpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _permissionRequirement = permissionRequirement;

        }
        /// <summary>
        /// wx.login登陆成功之后发送的请求
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> OnLogin([FromBody] FMWxPost entity)
        {
            var jm = new WebApiCallBack();

            var client = _weChatApiHttpClientFactory.CreateWxOpenClient();
            var accessToken = WeChatCacheAccessTokenHelper.GetWxOpenAccessToken();
            var request = new SnsJsCode2SessionRequest();
            request.JsCode = entity.code;
            request.AccessToken = accessToken;

            var response = await client.ExecuteSnsJsCode2SessionAsync(request, HttpContext.RequestAborted);
            if (response.ErrorCode == (int)WeChatReturnCode.ReturnCode.请求成功)
            {
                using (await _mutex.LockAsync())
                {
                    var userInfo = await _userWeChatInfoServices.QueryByClauseAsync(p => p.openid == response.OpenId);
                    if (userInfo == null)
                    {
                        userInfo = new UserWeChatInfo();
                        userInfo.openid = response.OpenId;
                        userInfo.type = (int)GlobalEnumVars.UserAccountTypes.微信小程序;
                        userInfo.sessionKey = response.SessionKey;
                        userInfo.gender = 1;
                        userInfo.createTime = DateTime.Now;

                        await _userWeChatInfoServices.InsertAsync(userInfo);
                    }
                    else
                    {
                        if (userInfo.sessionKey != response.SessionKey)
                        {
                            await _userWeChatInfoServices.UpdateAsync(
                                p => new UserWeChatInfo() { sessionKey = response.SessionKey, updateTime = DateTime.Now },
                                p => p.openid == userInfo.openid);
                        }
                    }

                    if (userInfo is { userId: > 0 })
                    {
                        var user = await _userServices.QueryByClauseAsync(p => p.id == userInfo.userId);
                        if (user != null)
                        {
                            var claims = new List<Claim> {
                                new Claim(ClaimTypes.Name, user.nickName),
                                new Claim(JwtRegisteredClaimNames.Jti, user.id.ToString()),
                                new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(20).ToString(CultureInfo.InvariantCulture)) };

                            //用户标识
                            var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                            identity.AddClaims(claims);
                            jm.status = true;
                            jm.data = new
                            {
                                auth = JwtToken.BuildJwtToken(claims.ToArray(), _permissionRequirement),
                                user
                            };
                            jm.otherData = response.OpenId;

                            //录入登录日志
                            var log = new UserLog();
                            log.userId = user.id;
                            log.state = (int)GlobalEnumVars.UserLogTypes.登录;
                            log.ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress != null ? _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() : "127.0.0.1";
                            log.createTime = DateTime.Now;
                            log.parameters = GlobalEnumVars.UserLogTypes.登录.ToString();
                            await _userLogServices.InsertAsync(log);

                            return jm;
                        }
                    }
                }
                jm.status = true;
                jm.data = response.OpenId;
                jm.otherData = response.OpenId;
                //jm.methodDescription = JsonConvert.SerializeObject(sessionBag);
                jm.msg = "OK";
            }
            else
            {
                jm.msg = response.ErrorMessage;
            }

            return jm;
        }
    }
}
