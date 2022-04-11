using FuFuShop.Model.Entities;
using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     支付宝支付 接口实现
    /// </summary>
    public class AliPayServices : BaseServices<Setting>, IAliPayServices
    {
        public AliPayServices(IWeChatPayRepository dal)
        {
            BaseDal = dal;
        }

        /// <summary>
        ///     发起支付
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public WebApiCallBack PubPay(BillPayments entity)
        {
            var jm = new WebApiCallBack();
            return jm;
        }
    }
}