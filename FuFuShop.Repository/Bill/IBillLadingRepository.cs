
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     提货单表 工厂接口
    /// </summary>
    public interface IBillLadingRepository : IBaseRepository<BillLading>
    {
        /// <summary>
        ///     添加提货单
        /// </summary>
        /// <returns></returns>
        Task<WebApiCallBack> AddData(string orderId, int storeId, string name, string mobile);
    }
}