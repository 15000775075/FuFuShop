using FuFuShop.Model.Entities;
using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     支付宝支付 服务工厂接口
    /// </summary>
    public interface IAliPayServices : IBaseServices<Setting>
    {
        /// <summary>
        ///     发起支付
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        WebApiCallBack PubPay(BillPayments entity);
    }
}