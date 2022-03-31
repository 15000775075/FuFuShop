
using CoreCms.Net.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 订单记录表 接口实现
    /// </summary>
    public class OrderLogRepository : BaseRepository<OrderLog>, IOrderLogRepository
    {
        public OrderLogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


    }
}
