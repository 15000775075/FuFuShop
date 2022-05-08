using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Common.Extensions;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using FuFuShop.Services.Shop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace FuFuShop.Controllers
{
    /// <summary>
    /// 订单调用接口数据
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IHttpContextUser _user;
        private readonly IOrderServices _orderServices;
        private readonly IAreaServices _areaServices;
        private readonly ILogisticsServices _logisticsServices;
        private readonly IBillAftersalesServices _aftersalesServices;
        private readonly ISettingServices _settingServices;
        private readonly IShipServices _shipServices;
        private readonly IBillReshipServices _reshipServices;


        /// <summary>
        /// 构造函数
        /// </summary>
        public OrderController(IHttpContextUser user
        , IOrderServices orderServices
        , IAreaServices areaServices,
            ILogisticsServices logisticsServices    ,
            IBillAftersalesServices aftersalesServices ,
            ISettingServices settingServices ,
            IShipServices shipServices  ,
            IBillReshipServices reshipServices

)
        {
            _user = user;
            _orderServices = orderServices;
            _areaServices = areaServices;
            _logisticsServices = logisticsServices;
            _aftersalesServices = aftersalesServices;
            _settingServices = settingServices;
            _shipServices = shipServices;
            _reshipServices = reshipServices;

        }

        #region 创建订单==================================================
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> CreateOrder([FromBody] CreateOrder entity)
        {
            var jm = new WebApiCallBack();

            var type = entity.receiptType;
            if (type == (int)GlobalEnumVars.OrderReceiptType.Logistics || type == (int)GlobalEnumVars.OrderReceiptType.IntraCityService)
            {
                //收货地址id
                if (entity.ushipId == 0)
                {
                    jm.data = 13001;
                    jm.msg = GlobalErrorCodeVars.Code13001;
                }
            }
            else if (type == (int)GlobalEnumVars.OrderReceiptType.SelfDelivery)
            {
                //提货门店
                if (entity.storeId == 0)
                {
                    jm.data = 13001;
                    jm.msg = GlobalErrorCodeVars.Code13001;
                }
                //提货人姓名 提货人电话
                if (string.IsNullOrEmpty(entity.ladingName))
                {
                    jm.data = 13001;
                    jm.msg = "请输入姓名";
                }
                if (string.IsNullOrEmpty(entity.ladingMobile))
                {
                    jm.data = 13001;
                    jm.msg = "请输入电话";
                }
            }
            else
            {
                jm.data = 13001;
                jm.msg = "未查询到配送方式";
            }

            if (string.IsNullOrEmpty(entity.cartIds))
            {
                jm.data = 10000;
                jm.msg = GlobalErrorCodeVars.Code10000;
            }
            jm = await _orderServices.ToAdd(_user.ID, entity.orderType, entity.cartIds, entity.receiptType,
                entity.ushipId, entity.storeId, entity.ladingName, entity.ladingMobile, entity.memo,
                entity.point, entity.couponCode, entity.source, entity.scene, entity.taxType, entity.taxName,
                entity.taxCode, entity.objectId, entity.teamId);
            jm.otherData = entity;

            return jm;
        }
        #endregion
        #region 订单预览==================================================
        /// <summary>
        /// 订单预览
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> OrderDetails([FromBody] FMStringId entity)
        {
            var jm = new WebApiCallBack();
            var userId = _user.ID;
            jm = await _orderServices.GetOrderInfoByOrderId(entity.id, userId);
            return jm;
        }
        #endregion
        #region 获取订单不同状态的数量==================================================
        /// <summary>
        /// 获取订单不同状态的数量
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> GetOrderStatusNum([FromBody] GetOrderStatusNumPost entity)
        {
            var jm = new WebApiCallBack();

            if (string.IsNullOrEmpty(entity.ids))
            {
                jm.msg = "请提交要查询的订单统计类型";
            }
            var ids = CommonHelper.StringToIntArray(entity.ids);
            jm = await _orderServices.GetOrderStatusNum(_user.ID, ids, entity.isAfterSale);
            return jm;

        }
        #endregion
        #region 获取个人订单列表=======================================================

        /// <summary>
        /// 获取个人订单列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> GetOrderList([FromBody] GetOrderListPost entity)
        {
            var jm = await _orderServices.GetOrderList(entity.key,entity.startTime,entity.endTime, entity.status, _user.ID, entity.page, entity.limit);
            return jm;
        }

        #endregion

        #region 取消订单====================================================

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> CancelOrder([FromBody] FMStringId entity)
        {
            var jm = new WebApiCallBack();

            if (string.IsNullOrEmpty(entity.id))
            {
                jm.msg = "请提交要取消的订单号";
                return jm;
            }
            var ids = entity.id.Split(",");
            jm = await _orderServices.CancelOrder(ids, _user.ID);
            return jm;

        }
        #endregion

        #region 删除订单====================================================

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> DeleteOrder([FromBody] FMStringId entity)
        {
            var jm = new WebApiCallBack();

            if (string.IsNullOrEmpty(entity.id))
            {
                jm.msg = "请提交要取消的订单号";
                return jm;
            }
            var ids = entity.id.Split(",");
            jm.status = await _orderServices.DeleteAsync(p => ids.Contains(p.orderId) && p.userId == _user.ID);
            jm.msg = jm.status ? "删除成功" : "删除失败";
            return jm;

        }
        #endregion

        #region 确认签收订单====================================================
        /// <summary>
        /// 确认签收订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> OrderConfirm([FromBody] FMStringId entity)
        {
            var jm = new WebApiCallBack();

            if (string.IsNullOrEmpty(entity.id))
            {
                jm.msg = "请提交要确认签收的订单号";
                return jm;
            }
            jm = await _orderServices.ConfirmOrder(entity.id, Convert.ToInt32(entity.data));
            return jm;

        }
        #endregion
        #region 前台物流查询接口=======================================================

        /// <summary>
        /// 前台物流查询接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> LogisticsByApi([FromBody] FMApiLogisticsByApiPost entity)
        {
            var jm = new WebApiCallBack();

            if (string.IsNullOrEmpty(entity.code) || string.IsNullOrEmpty(entity.no))
            {
                jm.msg = GlobalErrorCodeVars.Code13225;
                return jm;
            }

            var systemLogistics = SystemSettingDictionary.GetSystemLogistics();
            foreach (var p in systemLogistics)
            {
                if (entity.code == p.sKey)
                {
                    jm.msg = p.sDescription + "不支持轨迹查询";
                    return jm;
                }
            }

            jm = await _logisticsServices.ExpressPoll(entity.code, entity.no, entity.mobile);
            return jm;
        }

        #endregion


        #region 添加售后单=======================================================

        /// <summary>
        /// 添加售后单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> AddAftersales([FromBody] ToAddBillAfterSalesPost entity)
        {
            var jm = new WebApiCallBack();

            jm.otherData = entity;
            if (string.IsNullOrEmpty(entity.orderId))
            {
                jm.msg = GlobalErrorCodeVars.Code13100;
                jm.code = 13100;
                return jm;
            }
            if (entity.type == 0)
            {
                jm.msg = GlobalErrorCodeVars.Code10051;
                jm.code = 10051;
                return jm;
            }
            jm = await _aftersalesServices.ToAdd(_user.ID, entity.orderId, entity.type, entity.items, entity.images,
                entity.reason, entity.refund);
            return jm;

        }

        #endregion

        #region 获取售后单列表=======================================================

        /// <summary>
        /// 获取售后单列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> AftersalesList([FromBody] FMPageByStringId entity)
        {
            var jm = new WebApiCallBack();

            jm.status = true;
            jm.msg = "数据获取成功";

            var where = PredicateBuilder.True<BillAftersales>();
            where = where.And(p => p.userId == _user.ID);
            if (!string.IsNullOrEmpty(entity.order))
            {
                where = where.And(p => p.orderId == entity.id);
            }
            var data = await _aftersalesServices.QueryPageAsync(where, p => p.createTime, OrderByType.Desc, entity.page, entity.limit);

            jm.data = new
            {
                list = data,
                page = data.PageIndex,
                totalPage = data.TotalPages,
                hasNextPage = data.HasNextPage
            };

            return jm;

        }

        #endregion

        #region 获取单个售后单详情

        /// <summary>
        /// 获取售后单列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> Aftersalesinfo([FromBody] FMStringId entity)
        {
            var jm = new WebApiCallBack { status = true, msg = "数据获取成功" };

            var info = await _aftersalesServices.GetInfo(entity.id, _user.ID);

            var allConfigs = await _settingServices.GetConfigDictionaries();

            var reshipAddress = CommonHelper.GetConfigDictionary(allConfigs, SystemSettingConstVars.ReshipAddress);
            var reshipArea = string.Empty;
            var reshipId = CommonHelper.GetConfigDictionary(allConfigs, SystemSettingConstVars.ReshipAreaId).ObjectToInt(0);
            if (reshipId > 0)
            {
                var result = await _areaServices.GetAreaFullName(reshipId);
                if (result.status)
                {
                    reshipArea = result.data.ToString();
                }
            }
            var reshipMobile = CommonHelper.GetConfigDictionary(allConfigs, SystemSettingConstVars.ReshipMobile);
            var reshipName = CommonHelper.GetConfigDictionary(allConfigs, SystemSettingConstVars.ReshipName);
            var reship = new
            {
                reshipAddress,
                reshipArea,
                reshipMobile,
                reshipName
            };

            jm.data = new
            {
                info,
                reship
            };
            return jm;

        }

        #endregion

        #region 提交售后发货快递信息

        /// <summary>
        /// 提交售后发货快递信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> SendReship([FromBody] FMBillReshipForSendReshipPost entity)
        {
            var jm = new WebApiCallBack();

            if (string.IsNullOrEmpty(entity.reshipId))
            {
                jm.data = jm.msg = GlobalErrorCodeVars.Code13212;
                return jm;
            }
            else if (string.IsNullOrEmpty(entity.logiCode))
            {
                jm.data = jm.msg = GlobalErrorCodeVars.Code13213;
                return jm;
            }
            else if (string.IsNullOrEmpty(entity.logiNo))
            {
                jm.data = jm.msg = GlobalErrorCodeVars.Code13214;
                return jm;
            }


            var model = await _reshipServices.QueryByIdAsync(entity.reshipId);
            if (model == null)
            {
                jm.data = jm.msg = GlobalErrorCodeVars.Code13211;
                return jm;
            }

            var up = await _reshipServices.UpdateAsync(
                p => new BillReship()
                {
                    logiCode = entity.logiCode,
                    logiNo = entity.logiNo,
                    status = (int)GlobalEnumVars.BillReshipStatus.运输中
                }, p => p.reshipId == entity.reshipId);

            jm.status = true;
            jm.msg = "数据保存成功";

            return jm;
        }

        #endregion
    }
}