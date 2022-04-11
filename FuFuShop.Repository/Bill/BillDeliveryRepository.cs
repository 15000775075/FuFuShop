using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;

namespace FuFuShop.Repository.Bill
{
    /// <summary>
    /// 发货单表 接口实现
    /// </summary>
    public class BillDeliveryRepository : BaseRepository<BillDelivery>, IBillDeliveryRepository
    {
        public BillDeliveryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        /// 发货单统计7天统计
        /// </summary>
        /// <returns></returns>
        public async Task<List<StatisticsOut>> Statistics()
        {
            var dt = DateTime.Now.AddDays(-8);

            var list = await DbClient.Queryable<BillDelivery>()
                .Where(p => p.createTime >= dt)
                .Select(it => new
                {
                    it.deliveryId,
                    createTime = it.createTime.Date
                })
                .MergeTable()
                .GroupBy(it => it.createTime)
                .Select(it => new StatisticsOut { day = it.createTime.ToString("yyyy-MM-dd"), nums = SqlFunc.AggregateCount(it.deliveryId) })
                .ToListAsync();

            return list;
        }

    }
}
