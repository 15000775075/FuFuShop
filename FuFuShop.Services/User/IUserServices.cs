using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services.User
{
    /// <summary>
    ///     用户表 服务工厂接口
    /// </summary>
    public interface IUserServices : IBaseServices<FuFuShopUser>
    {
        /// <summary>
        ///     按天统计新会员
        /// </summary>
        /// <returns></returns>
        Task<List<StatisticsOut>> Statistics(int day);


        /// <summary>
        ///     按天统计当天下单活跃会员
        /// </summary>
        /// <returns></returns>
        Task<List<StatisticsOut>> StatisticsOrder(int day);

    }
}