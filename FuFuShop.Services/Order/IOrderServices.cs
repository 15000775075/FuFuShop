using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     订单表 服务工厂接口
    /// </summary>
    public interface IOrderServices : IBaseServices<Order>
    {

        /// <summary>
        ///     创建订单
        /// </summary>
        /// <param name="userId">用户序列</param>
        /// <param name="orderType">订单类型，1是普通订单，2是拼团订单</param>
        /// <param name="cartIds">购物车货品序列</param>
        /// <param name="receiptType">收货方式,1快递物流，2同城配送，3门店自提</param>
        /// <param name="ushipId">用户地址库序列</param>
        /// <param name="storeId">门店序列</param>
        /// <param name="ladingName">提货人姓名</param>
        /// <param name="ladingMobile">提货人联系方式</param>
        /// <param name="memo">备注</param>
        /// <param name="point">积分</param>
        /// <param name="couponCode">优惠券码</param>
        /// <param name="source">来源平台</param>
        /// <param name="scene">场景值（一般小程序才有）</param>
        /// <param name="taxType">发票信息</param>
        /// <param name="taxName">发票抬头</param>
        /// <param name="taxCode">发票税务编码</param>
        /// <param name="objectId">关联非普通订单营销功能的序列</param>
        /// <param name="teamId">拼团订单分组序列</param>
        /// <returns></returns>
        Task<WebApiCallBack> ToAdd(int userId, int orderType, string cartIds, int receiptType, int ushipId, int storeId,
            string ladingName, string ladingMobile, string memo, int point, string couponCode,
            int source, int scene, int taxType, string taxName, string taxCode, int objectId, int teamId);

        /// <summary>
        ///     获取订单信息
        /// </summary>
        /// <returns></returns>
        Task<WebApiCallBack> GetOrderInfoByOrderId(string id, int userId = 0, int aftersaleLevel = 0);



        /// <summary>
        ///     订单数量统计
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> OrderCount(int type = 0, int userId = 0);


        /// <summary>
        ///     订单支付
        /// </summary>
        /// <param name="orderId">订单编号</param>
        /// <param name="paymentCode">支付方式</param>
        /// <param name="billPaymentInfo">支付单据</param>
        /// <returns></returns>
        Task<WebApiCallBack> Pay(string orderId, string paymentCode, BillPayments billPaymentInfo);



        /// <summary>
        ///     判断订单是否可以进行评论
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebApiCallBack> IsOrderComment(string orderId, int userId);

        /// <summary>
        ///     获取订单不同状态的数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ids"></param>
        /// <param name="isAfterSale"></param>
        /// <returns></returns>
        Task<WebApiCallBack> GetOrderStatusNum(int userId, int[] ids, bool isAfterSale = false);

        /// <summary>
        ///     获取订单列表微信小程序
        /// </summary>
        /// <returns></returns>
        Task<WebApiCallBack> GetOrderList(int status = -1, int userId = 0, int page = 1, int limit = 5);


        /// <summary>
        ///     取消订单
        /// </summary>
        /// <returns></returns>
        Task<WebApiCallBack> CancelOrder(string[] ids, int userId = 0);

        /// <summary>
        ///     确认签收订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebApiCallBack> ConfirmOrder(string orderId, int userId = 0);

    }
}