using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;

namespace FuFuShop.Repository.Bill
{
    /// <summary>
    ///     发货单表 工厂接口
    /// </summary>
    public interface IBillDeliveryRepository : IBaseRepository<BillDelivery>
    {
        /// <summary>
        ///     发货单统计7天统计
        /// </summary>
        /// <returns></returns>
        Task<List<StatisticsOut>> Statistics();
    }
}