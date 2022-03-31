using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     订单记录表 工厂接口
    /// </summary>
    public interface IOrderLogRepository : IBaseRepository<OrderLog>
    {
    }
}