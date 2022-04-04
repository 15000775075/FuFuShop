using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using FuFuShop.Services.Shop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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


        /// <summary>
        /// 构造函数
        /// </summary>
        public OrderController(IHttpContextUser user
        , IOrderServices orderServices
        , IAreaServices areaServices

)
        {
            _user = user;
            _orderServices = orderServices;
            _areaServices = areaServices;

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
        //#region 获取订单不同状态的数量==================================================
        ///// <summary>
        ///// 获取订单不同状态的数量
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public async Task<WebApiCallBack> GetOrderStatusNum([FromBody] GetOrderStatusNumPost entity)
        //{
        //    var jm = new WebApiCallBack();

        //    if (string.IsNullOrEmpty(entity.ids))
        //    {
        //        jm.msg = "请提交要查询的订单统计类型";
        //    }
        //    var ids = CommonHelper.StringToIntArray(entity.ids);
        //    jm = await _orderServices.GetOrderStatusNum(_user.ID, ids, entity.isAfterSale);
        //    return jm;

        //}
        //#endregion
        //#region 获取个人订单列表=======================================================

        ///// <summary>
        ///// 获取个人订单列表
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public async Task<WebApiCallBack> GetOrderList([FromBody] GetOrderListPost entity)
        //{
        //    var jm = await _orderServices.GetOrderList(entity.status, _user.ID, entity.page, entity.limit);
        //    return jm;
        //}

        //#endregion

        //#region 取消订单====================================================

        ///// <summary>
        ///// 取消订单
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public async Task<WebApiCallBack> CancelOrder([FromBody] FMStringId entity)
        //{
        //    var jm = new WebApiCallBack();

        //    if (string.IsNullOrEmpty(entity.id))
        //    {
        //        jm.msg = "请提交要取消的订单号";
        //        return jm;
        //    }
        //    var ids = entity.id.Split(",");
        //    jm = await _orderServices.CancelOrder(ids, _user.ID);
        //    return jm;

        //}
        //#endregion

        //#region 删除订单====================================================

        ///// <summary>
        ///// 删除订单
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public async Task<WebApiCallBack> DeleteOrder([FromBody] FMStringId entity)
        //{
        //    var jm = new WebApiCallBack();

        //    if (string.IsNullOrEmpty(entity.id))
        //    {
        //        jm.msg = "请提交要取消的订单号";
        //        return jm;
        //    }
        //    var ids = entity.id.Split(",");
        //    jm.status = await _orderServices.DeleteAsync(p => ids.Contains(p.orderId) && p.userId == _user.ID);
        //    jm.msg = jm.status ? "删除成功" : "删除失败";
        //    return jm;

        //}
        //#endregion

        //#region 确认签收订单====================================================
        ///// <summary>
        ///// 确认签收订单
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public async Task<WebApiCallBack> OrderConfirm([FromBody] FMStringId entity)
        //{
        //    var jm = new WebApiCallBack();

        //    if (string.IsNullOrEmpty(entity.id))
        //    {
        //        jm.msg = "请提交要确认签收的订单号";
        //        return jm;
        //    }
        //    jm = await _orderServices.ConfirmOrder(entity.id, Convert.ToInt32(entity.data));
        //    return jm;

        //}
        //#endregion
        //#region 前台物流查询接口=======================================================

        ///// <summary>
        ///// 前台物流查询接口
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public async Task<WebApiCallBack> LogisticsByApi([FromBody] FMApiLogisticsByApiPost entity)
        //{
        //    var jm = new WebApiCallBack();

        //    if (string.IsNullOrEmpty(entity.code) || string.IsNullOrEmpty(entity.no))
        //    {
        //        jm.msg = GlobalErrorCodeVars.Code13225;
        //        return jm;
        //    }

        //    var systemLogistics = SystemSettingDictionary.GetSystemLogistics();
        //    foreach (var p in systemLogistics)
        //    {
        //        if (entity.code == p.sKey)
        //        {
        //            jm.msg = p.sDescription + "不支持轨迹查询";
        //            return jm;
        //        }
        //    }

        //    jm = await _logisticsServices.ExpressPoll(entity.code, entity.no, entity.mobile);
        //    return jm;
        //}

        //#endregion
    }
}