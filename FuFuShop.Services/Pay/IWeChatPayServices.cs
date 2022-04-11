using FuFuShop.Model.Entities;
using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     微信支付调用 服务工厂接口
    /// </summary>
    public interface IWeChatPayServices : IBaseServices<Setting>
    {
        /// <summary>
        ///     发起支付
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        Task<WebApiCallBack> PubPay(BillPayments entity);


        /// <summary>
        ///     用户退款
        /// </summary>
        /// <param name="refundInfo">退款单数据</param>
        /// <param name="paymentInfo">支付单数据</param>
        /// <returns></returns>
        Task<WebApiCallBack> Refund(BillRefund refundInfo, BillPayments paymentInfo);
    }
}