using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services.Shop
{
    /// <summary>
    ///     物流公司表 服务工厂接口
    /// </summary>
    public interface ILogisticsServices : IBaseServices<Logistics>
    {

        /// <summary>
        ///     根据物流编码取物流名称等信息
        /// </summary>
        /// <param name="logiCode">物流编码</param>
        /// <returns></returns>
        Task<WebApiCallBack> GetLogiInfo(string logiCode);


        /// <summary>
        ///     通过接口
        /// </summary>
        Task<AdminUiCallBack> DoUpdateCompany();


        /// <summary>
        ///     通过接口获取快递信息
        /// </summary>
        /// <param name="com">来源</param>
        /// <param name="number">编号</param>
        /// <param name="phone">手机号码</param>
        /// <returns></returns>
        Task<WebApiCallBack> ExpressPoll(string com, string number, string phone);
    }
}