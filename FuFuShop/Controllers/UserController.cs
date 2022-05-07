using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Auth;
using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using FuFuShop.Services.Good;
using FuFuShop.Services.Shop;
using FuFuShop.Services.WeChat;
using FuFuShop.WeChat.Enums;
using FuFuShop.WeChat.Services.HttpClients;
using FuFuShop.WeChat.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nito.AsyncEx;
using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;
using SqlSugar;
using SqlSugar.Extensions;
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
        private readonly IGoodsBrowsingServices _goodsBrowsingServices;
        private readonly IGoodsCollectionServices _goodsCollectionServices;
        private readonly IGoodsServices _goodsServices;
        private readonly IUserShipServices _userShipServices;
        private readonly IAreaServices _areaServices;
        private readonly IBillPaymentsServices _billPaymentsServices;
        private readonly IGoodsCommentServices _goodsCommentServices;
        private readonly ICartServices _cartServices;


        public UserController(
            IHttpContextUser user,
            IUserServices userServices,
            IUserWeChatInfoServices userWeChatInfoServices,
            IWeChatApiHttpClientFactory weChatApiHttpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IUserLogServices userLogServices,
            PermissionRequirement permissionRequirement,
            IOrderServices orderServices,
            IGoodsBrowsingServices goodsBrowsingServices,
            IGoodsCollectionServices goodsCollectionServices,
            IGoodsServices goodsServices,
            IUserShipServices userShipServices,
            IAreaServices areaServices,
            IBillPaymentsServices billPaymentsServices,
            IGoodsCommentServices goodsCommentServices,
            ICartServices cartServices
            )
        {
            _user = user;
            _userLogServices = userLogServices;
            _userServices = userServices;
            _userWeChatInfoServices = userWeChatInfoServices;
            _weChatApiHttpClientFactory = weChatApiHttpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _permissionRequirement = permissionRequirement;
            _orderServices = orderServices;
            _goodsBrowsingServices = goodsBrowsingServices;
            _goodsCollectionServices = goodsCollectionServices;
            _goodsServices = goodsServices;
            _userShipServices = userShipServices;
            _areaServices = areaServices;
            _billPaymentsServices = billPaymentsServices;
            _goodsCommentServices = goodsCommentServices;
            _cartServices = cartServices;

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
            //订单数量
            var orderCount = await _orderServices.OrderCount(0, user.id);
            //足迹
            var footPrintCount = await _goodsBrowsingServices.GetCountAsync(p => p.userId == user.id);
            //收藏
            var collectionCount = await _goodsCollectionServices.GetCountAsync(p => p.userId == user.id);


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
                orderCount,
                footPrintCount,
                collectionCount
            };
            return jm;
        }
        #endregion

        #region 商品取消/添加收藏
        /// <summary>
        /// 商品取消/添加收藏
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> GoodsCollectionCreateOrDelete([FromBody] FMIntId entity)
        {
            var jm = new WebApiCallBack();

            var collection = await _goodsCollectionServices.QueryByClauseAsync(p => p.goodsId == entity.id && p.userId == _user.ID);
            if (collection == null)
            {
                var goods = await _goodsServices.QueryByIdAsync(entity.id);
                if (goods == null)
                {
                    jm.msg = GlobalErrorCodeVars.Code17001;
                    return jm;
                }

                collection = new Model.Entities.GoodsCollection()
                {
                    goodsId = goods.id,
                    userId = _user.ID,
                    goodsName = goods.name,
                    createTime = DateTime.Now,
                };
                await _goodsCollectionServices.InsertAsync(collection);
                jm.msg = GlobalErrorCodeVars.Code17002;
            }
            else
            {
                await _goodsCollectionServices.DeleteAsync(collection);
                jm.msg = GlobalErrorCodeVars.Code17003;
            }
            jm.status = true;

            return jm;
        }


        #endregion

        #region 获取用户获取用户默认收货地址
        /// <summary>
        /// 获取用户获取用户默认收货地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> GetUserDefaultShip()
        {
            var jm = new WebApiCallBack();

            var ship = await _userShipServices.QueryByClauseAsync(p => p.isDefault && p.userId == _user.ID) ?? await _userShipServices.QueryByClauseAsync(p => p.userId == _user.ID, p => p.id, OrderByType.Desc);

            if (ship != null)
            {
                var fullName = await _areaServices.GetAreaFullName(ship.areaId);
                if (fullName.status)
                {
                    ship.areaName = fullName.data.ToString();
                }
            }

            jm.status = true;
            jm.data = ship;

            return jm;
        }
        #endregion

        #region 设置默认地址
        /// <summary>
        /// 设置默认地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> SetDefShip([FromBody] FMIntId entity)
        {
            var jm = new WebApiCallBack();

            var ship = await _userShipServices.QueryByClauseAsync(p => p.id == entity.id && p.userId == _user.ID);
            if (ship != null)
            {
                //没有默认的直接设置为默认
                ship.isDefault = true;
                var result = await _userShipServices.UpdateAsync(ship);
                jm.status = result.code == 0;
                jm.msg = jm.status ? "保存成功" : "保存失败";
            }
            else
            {
                jm.msg = "该地址不存在";
            }

            return jm;
        }

        #endregion

        #region 获取用户的收货地址列表
        /// <summary>
        /// 获取用户的收货地址列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> GetUserShip()
        {
            var jm = new WebApiCallBack();

            var ship = await _userShipServices.QueryListByClauseAsync(p => p.userId == _user.ID, p => p.isDefault, OrderByType.Desc);
            if (ship.Any())
            {
                ship.ForEach(Action);
            }
            jm.status = true;
            jm.data = ship;

            return jm;
        }

        private async void Action(UserShip p)
        {
            var fullName = await _areaServices.GetAreaFullName(p.areaId);
            if (fullName.status)
            {
                p.areaName = fullName.data.ToString();
            }
        }

        #endregion

        #region 保存用户地址
        /// <summary>
        /// 保存用户地址
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> SaveUserShip([FromBody] SaveUserShipPost entity)
        {
            var jm = new WebApiCallBack();

            if (entity.id > 0)
            {
                //判断是否存在默认数据
                if (entity.isDefault != 1)
                {
                    if (await _userShipServices.ExistsAsync(p => p.userId == _user.ID && p.isDefault == true && p.id != entity.id) == false) entity.isDefault = 1;
                }
                var userShip = new UserShip();
                userShip.id = entity.id;
                userShip.userId = _user.ID;
                userShip.areaId = entity.areaId;
                userShip.isDefault = entity.isDefault == 1;
                userShip.name = entity.name;
                userShip.address = entity.address;
                userShip.mobile = entity.mobile;
                userShip.updateTime = DateTime.Now;
                var ship = await _userShipServices.UpdateAsync(userShip);
                jm.status = true;
                jm.data = ship;
                jm.msg = "地址保存成功";
            }
            else
            {
                //判断是否存在默认数据
                if (entity.isDefault != 1)
                {
                    if (await _userShipServices.ExistsAsync(p => p.userId == _user.ID && p.isDefault == true) == false) entity.isDefault = 1;
                }
                var userShip = new UserShip();
                userShip.userId = _user.ID;
                userShip.areaId = entity.areaId;
                userShip.isDefault = entity.isDefault == 1;
                userShip.name = entity.name;
                userShip.address = entity.address;
                userShip.mobile = entity.mobile;
                userShip.createTime = DateTime.Now;
                var ship = await _userShipServices.InsertAsync(userShip);
                jm.status = true;
                jm.data = ship;
                jm.msg = "地址保存成功";
            }

            return jm;
        }

        #endregion

        #region 获取用户单个地址详情
        /// <summary>
        /// 获取用户单个地址详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> GetShipDetail([FromBody] FMIntId entity)
        {
            var jm = new WebApiCallBack();

            var ship = await _userShipServices.QueryByClauseAsync(p => p.userId == _user.ID && p.id == entity.id);
            if (ship != null)
            {
                //var areas = _areaServices.FindListAsync();
                var fullName = await _areaServices.GetAreaFullName(ship.areaId);
                if (fullName.status)
                {
                    ship.areaName = fullName.data.ToString();
                }
            }
            jm.status = true;
            jm.data = ship;

            return jm;
        }
        #endregion

        #region 收货地址删除
        /// <summary>
        /// 收货地址删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> RemoveShip([FromBody] FMIntId entity)
        {
            var jm = new WebApiCallBack();

            jm.status = await _userShipServices.DeleteAsync(p => p.userId == _user.ID && p.id == entity.id);
            jm.msg = jm.status ? GlobalConstVars.DeleteSuccess : GlobalConstVars.DeleteFailure;

            if (jm.status)
            {
                //如果只有一个地址了，默认将最后一个剩余的地址设置为默认。
                var anySum = await _userShipServices.GetCountAsync(p => p.userId == _user.ID);
                if (anySum == 1)
                {
                    await _userShipServices.UpdateAsync(p => new UserShip() { isDefault = true }, p => p.userId == _user.ID);
                }
            }
            return jm;
        }
        #endregion

        #region 支付

        /// <summary>
        /// 支付
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> Pay([FromBody] PayPost entity)
        {
            var jm = new WebApiCallBack();

            if (string.IsNullOrEmpty(entity.ids))
            {
                jm.code = 13100;
                jm.msg = GlobalErrorCodeVars.Code13100;
            }
            else if (string.IsNullOrEmpty(entity.payment_code))
            {
                jm.code = 10055;
                jm.msg = GlobalErrorCodeVars.Code10055;
            }
            else if (entity.payment_type == 0)
            {
                jm.code = 10051;
                jm.msg = GlobalErrorCodeVars.Code10051;
            }
            //生成支付单,并发起支付
            jm = await _billPaymentsServices.Pay(entity.ids, entity.payment_code, _user.ID, entity.payment_type,
                entity.@params);

            return jm;
        }

        #endregion

        #region 订单评价

        /// <summary>
        /// 订单评价
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> OrderEvaluate([FromBody] OrderEvaluatePost entity)
        {
            var jm = new WebApiCallBack();

            if (string.IsNullOrEmpty(entity.orderId))
            {
                jm.code = 13100;
                jm.msg = GlobalErrorCodeVars.Code13100;
            }
            else if (entity.items == null || entity.items.Count == 0)
            {
                jm.code = 10051;
                jm.msg = GlobalErrorCodeVars.Code10051;
            }
            jm = await _goodsCommentServices.AddComment(entity.orderId, entity.items, _user.ID);
            jm.otherData = entity;
            return jm;
        }

        #endregion

        #region 取得商品收藏记录（关注）
        /// <summary>
        /// 取得商品收藏记录（关注）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> GoodsCollectionList([FromBody] FMPageByIntId entity)
        {
            var jm = new WebApiCallBack();

            var goodsCollections = await _goodsCollectionServices.QueryPageAsync(p => p.userId == _user.ID, p => p.createTime, OrderByType.Desc, entity.page, entity.limit);

            foreach (var goodsCollection in goodsCollections)
            {
                goodsCollection.goods = await _goodsServices.GetGoodsDetial(goodsCollection.goodsId);
            }
            jm.status = true;
            jm.data = new
            {
                list = goodsCollections,
                count = goodsCollections.TotalCount,

            };
            return jm;

        }

        #endregion

        #region 添加商品收藏（关注）
        /// <summary>
        /// 添加商品收藏（关注）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> GoodsCollection([FromBody] FMIntId entity)
        {
            var jm = await _goodsCollectionServices.ToAdd(_user.ID, entity.id);
            return jm;
        }

        #endregion

        #region 取得商品浏览足迹
        /// <summary>
        /// 取得商品浏览足迹
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> Goodsbrowsing([FromBody] FMPageByIntId entity)
        {
            var jm = new WebApiCallBack();


            var otherData = entity.otherData.ObjToString();
            var obj = JsonConvert.DeserializeAnonymousType(otherData, new
            {
                startTime = Convert.ToDateTime(DateTime.Now.ToString("D").ToString()),
                endTime = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("D").ToString()).AddSeconds(-1),
            });

            var goodsBrowsings = await _goodsBrowsingServices.QueryPageAsync(p => p.userId == _user.ID && p.createTime > obj.startTime.ObjToDate() && p.createTime <= obj.endTime.ObjToDate(), p => p.createTime, OrderByType.Desc, entity.page, entity.limit);
            foreach (var goodsBrowsing in goodsBrowsings)
            {
                goodsBrowsing.goods = await _goodsServices.GetGoodsDetial(goodsBrowsing.goodsId);
            }


            var goodsBrowsings2 = goodsBrowsings.GroupBy(x => x.createTime.ToString("yyyy-MM-dd")).Select
           (grp => new FMGoodsBrowsing
           {
               Time = grp.Key.ToString(),
               Goods = grp.ToList()
           });


            jm.status = true;
            jm.data = new
            {
                list = goodsBrowsings2,
            };
            return jm;

        }

        #endregion

        #region 添加商品浏览足迹
        /// <summary>
        /// 添加商品浏览足迹
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> AddGoodsBrowsing([FromBody] FMIntId entity)
        {
            var jm = new WebApiCallBack();


            //获取数据
            var goods = await _goodsServices.QueryByIdAsync(entity.id);
            if (goods == null)
            {
                jm.msg = GlobalConstVars.DataisNo;
                return jm;
            }
            var goodsBrowsing = new GoodsBrowsing
            {
                goodsId = goods.id,
                userId = _user.ID,
                goodsName = goods.name,
                createTime = DateTime.Now,
                isdel = false
            };
            jm.status = await _goodsBrowsingServices.InsertAsync(goodsBrowsing) > 0;
            jm.msg = jm.status ? GlobalConstVars.InsertSuccess : GlobalConstVars.InsertFailure;

            return jm;
        }


        #endregion

        #region 删除商品浏览足迹
        /// <summary>
        /// 删除商品浏览足迹
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> DelGoodsBrowsing([FromBody] FMIntId entity)
        {
            var jm = new WebApiCallBack();

            jm.status = await _goodsBrowsingServices.DeleteAsync(p => p.userId == _user.ID && p.goodsId == entity.id);
            jm.msg = jm.status ? GlobalConstVars.DeleteSuccess : GlobalConstVars.DeleteFailure;

            return jm;
        }
        #endregion

        #region 获取购物车商品数量
        /// <summary>
        /// 获取购物车商品数量
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> GetCartNumber()
        {
            var jm = new WebApiCallBack();

            var count = await _cartServices.GetCountAsync(_user.ID);
            jm.status = true;
            jm.msg = jm.status ? GlobalConstVars.GetDataSuccess : GlobalConstVars.GetDataFailure;
            jm.data = count;

            return jm;
        }
        #endregion

    }
}
