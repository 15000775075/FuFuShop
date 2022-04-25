using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 公告表 接口实现
    /// </summary>
    public class NoticeRepository : BaseRepository<Notice>, INoticeRepository
    {
        public NoticeRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        ///     重写根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        public async Task<IPageList<Notice>> QueryPageAsync(Expression<Func<Notice, bool>> predicate,
            Expression<Func<Notice, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20)
        {
            RefAsync<int> totalCount = 0;
            var page = await DbClient.Queryable<Notice>()
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .WhereIF(predicate != null, predicate).Select(p => new Notice
                {
                    id = p.id,
                    title = p.title,
                    type = p.type,
                    sort = p.sort,
                    isDel = p.isDel,
                    createTime = p.createTime
                }).ToPageListAsync(pageIndex, pageSize, totalCount);
            var list = new PageList<Notice>(page, pageIndex, pageSize, totalCount);
            return list;
        }


        /// <summary>
        ///     获取列表首页用
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        public async Task<List<Notice>> QueryListAsync(Expression<Func<Notice, bool>> predicate,
            Expression<Func<Notice, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20)
        {
            var list = await DbClient.Queryable<Notice>().OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .WhereIF(predicate != null, predicate).Select(p => new Notice
                {
                    id = p.id,
                    title = p.title,
                    type = p.type,
                    sort = p.sort,
                    isDel = p.isDel,
                    createTime = p.createTime
                }).ToPageListAsync(pageIndex, pageSize);
            return list;
        }



    }
}
