using CoreCms.Net.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;

namespace FuFuShop.IRepository
{
    /// <summary>
    ///     发货单表 工厂接口
    /// </summary>
    public interface IBillDeliveryRepository : IBaseRepository<CoreCmsBillDelivery>
    {
        /// <summary>
        ///     发货单统计7天统计
        /// </summary>
        /// <returns></returns>
        Task<List<StatisticsOut>> Statistics();
    }
}