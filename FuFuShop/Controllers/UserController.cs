using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Auth;
using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using FuFuShop.Services.User;
using FuFuShop.Services.WeChat;
using FuFuShop.WeChat.Enums;
using FuFuShop.WeChat.Services.HttpClients;
using FuFuShop.WeChat.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IHttpContextUser _user;
        private readonly IOrderServices _orderServices;
        public UserController(
            IHttpContextUser user,
            IUserServices userServices,
            IUserWeChatInfoServices userWeChatInfoServices,
            IWeChatApiHttpClientFactory weChatApiHttpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IUserLogServices userLogServices,
            PermissionRequirement permissionRequirement,
            IOrderServices orderServices)
        {
            _user = user;
            _userLogServices = userLogServices;
            _userServices = userServices;
            _userWeChatInfoServices = userWeChatInfoServices;
            _weChatApiHttpClientFactory = weChatApiHttpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _permissionRequirement = permissionRequirement;
            _orderServices = orderServices;

        }

        #region wx.login登陆成功之后发送的请求=========================================================
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
                    var userWeChatInfo = await _userWeChatInfoServices.QueryByClauseAsync(p => p.openid == response.OpenId);
                    if (userWeChatInfo == null)
                    {


                        FuFuShopUser fuFuShopUser = new FuFuShopUser();
                        fuFuShopUser.userName = entity.userinfo.nickName != null ? entity.userinfo.nickName : "微信用户";
                        fuFuShopUser.mobile = "";
                        fuFuShopUser.sex = 0;
                        fuFuShopUser.avatarImage = entity.userinfo.avatarUrl != null ? entity.userinfo.avatarUrl : "";
                        fuFuShopUser.nickName = entity.userinfo.nickName != null ? entity.userinfo.nickName : "微信用户";
                        fuFuShopUser.balance = 0;
                        fuFuShopUser.parentId = 0;
                        fuFuShopUser.point = 0;
                        fuFuShopUser.grade = 0;
                        fuFuShopUser.createTime = DateTime.Now;
                        fuFuShopUser.status = 1;
                        fuFuShopUser.userWx = 0;
                        fuFuShopUser.isDelete = false;
                        fuFuShopUser.parentId = 0;
                        var userId = await _userServices.InsertAsync(fuFuShopUser);
                        if (userId > 0)
                        {

                            userWeChatInfo = new UserWeChatInfo();
                            userWeChatInfo.openid = response.OpenId;
                            userWeChatInfo.type = (int)GlobalEnumVars.UserAccountTypes.微信小程序;
                            userWeChatInfo.sessionKey = response.SessionKey;
                            userWeChatInfo.gender = 1;
                            userWeChatInfo.createTime = DateTime.Now;
                            userWeChatInfo.userId = userId;
                            await _userWeChatInfoServices.InsertAsync(userWeChatInfo);
                        }


                    }
                    else
                    {
                        if (userWeChatInfo.sessionKey != response.SessionKey)
                        {
                            await _userWeChatInfoServices.UpdateAsync(
                                p => new UserWeChatInfo() { sessionKey = response.SessionKey, updateTime = DateTime.Now },
                                p => p.openid == userWeChatInfo.openid);
                        }
                    }

                    if (userWeChatInfo is { userId: > 0 })
                    {
                        var user = await _userServices.QueryByClauseAsync(p => p.id == userWeChatInfo.userId);
                        if (user != null)
                        {
                            var claims = new List<Claim> {
                                new Claim(ClaimTypes.Name, user.nickName),
                                new Claim(JwtRegisteredClaimNames.Jti, user.id.ToString()),
                                new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(60).ToString(CultureInfo.InvariantCulture)) };

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
                jm.msg = "OK";
            }
            else
            {
                jm.msg = response.ErrorMessage;
            }

            return jm;
        }
        #endregion

        #region 获取用户信息
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> GetUserInfo()
        {
            var jm = new WebApiCallBack() { status = true };
            var user = await _userServices.QueryByIdAsync(_user.ID);
            if (user == null)
            {
                jm.status = false;
                jm.msg = "用户信息获取失败";
                jm.code = 14007;
                return jm;
            }
            ////订单数量
            //var orderCount = await _orderServices.OrderCount(0, user.id);
            ////足迹
            //var footPrintCount = await _goodsBrowsingServices.GetCountAsync(p => p.userId == user.id);
            ////收藏
            //var collectionCount = await _goodsCollectionServices.GetCountAsync(p => p.userId == user.id);


            jm.data = new
            {
                user.id,
                user.userName,
                user.mobile,
                user.sex,
                user.birthday,
                user.avatarImage,
                user.nickName,
                user.balance,
                user.point,
                user.grade,
                user.createTime,
                user.updataTime,
                user.status,
                user.parentId,
                user.passWord,
                // userCouponCount,
                // orderCount,
                // footPrintCount,
                // collectionCount
            };
            return jm;
        }
        #endregion

    }
}
