

using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 退货单明细表 接口实现
    /// </summary>
    public class BillReshipItemRepository : BaseRepository<BillReshipItem>, IBillReshipItemRepository
    {
        public BillReshipItemRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


    }
}
