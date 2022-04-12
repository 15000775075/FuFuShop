
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 退货单表 接口实现
    /// </summary>
    public class BillAftersalesRepository : BaseRepository<BillAftersales>, IBillAftersalesRepository
    {
        public BillAftersalesRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region 根据条件查询分页数据
        /// <summary>
        ///     根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        public async Task<IPageList<BillAftersales>> QueryPageAsync(Expression<Func<BillAftersales, bool>> predicate,
            Expression<Func<BillAftersales, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20)
        {
            RefAsync<int> totalCount = 0;
            var page = await DbClient.Queryable<BillAftersales, FuFuShopUser>((p, sc) => new JoinQueryInfos(
                     JoinType.Left, p.userId == sc.id))
                .Select((p, sc) => new BillAftersales
                {
                    aftersalesId = p.aftersalesId,
                    orderId = p.orderId,
                    userId = p.userId,
                    type = p.type,
                    refundAmount = p.refundAmount,
                    status = p.status,
                    reason = p.reason,
                    mark = p.mark,
                    createTime = p.createTime,
                    updateTime = p.updateTime,
                    userNickName = sc.nickName
                })
                .MergeTable()
                .Mapper(p => p.items, p => p.items.First().aftersalesId)
                .Mapper(p => p.images, p => p.images.First().aftersalesId)
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .WhereIF(predicate != null, predicate)
                .ToPageListAsync(pageIndex, pageSize, totalCount);

            if (page.Any())
            {
                var billAftersalesStatus = EnumHelper.EnumToList<GlobalEnumVars.BillAftersalesStatus>();
                foreach (var item in page)
                {
                    var statusModel = billAftersalesStatus.Find(p => p.value == item.status);
                    if (statusModel != null) item.statusName = statusModel.description;
                }
            }


            var list = new PageList<BillAftersales>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        #endregion

        #region 获取单个数据

        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <param name="aftersalesId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<BillAftersales> GetInfo(string aftersalesId, int userId = 0)
        {
            var model = userId > 0
                ? await DbClient.Queryable<BillAftersales>()
                    .Where(p => p.aftersalesId == aftersalesId && p.userId == userId).FirstAsync()
                : await DbClient.Queryable<BillAftersales>()
                    .Where(p => p.aftersalesId == aftersalesId).FirstAsync();

            if (model != null)
            {
                model.order = await DbClient.Queryable<Order>().Where(p => p.orderId == model.orderId).FirstAsync();
                model.images = await DbClient.Queryable<BillAftersalesImages>().Where(p => p.aftersalesId == aftersalesId).OrderBy(p => p.sortId).ToListAsync();
                model.items = await DbClient.Queryable<BillAftersalesItem>().Where(p => p.aftersalesId == aftersalesId).OrderBy(p => p.createTime).ToListAsync();
                model.billRefund = await DbClient.Queryable<BillRefund>().Where(p => p.aftersalesId == aftersalesId).FirstAsync();
                model.billReship = await DbClient.Queryable<BillReship>().Where(p => p.aftersalesId == aftersalesId).FirstAsync();
            }
            return model;
        }
        #endregion


    }
}
