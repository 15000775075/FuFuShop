using FuFuShop.IRepository;
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;
using System.Linq.Expressions;
using GoodsCollection = FuFuShop.Model.Entities.GoodsCollection;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 商品收藏表 接口实现
    /// </summary>
    public class GoodsCollectionRepository : BaseRepository<GoodsCollection>, IGoodsCollectionRepository
    {
        public GoodsCollectionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region 重写根据条件查询分页数据
        /// <summary>
        ///     重写根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        public async Task<IPageList<GoodsCollection>> QueryPageAsync(Expression<Func<GoodsCollection, bool>> predicate,
            Expression<Func<GoodsCollection, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20)
        {
            RefAsync<int> totalCount = 0;
            var page = await DbClient.Queryable<GoodsCollection>()
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .WhereIF(predicate != null, predicate)
                .Mapper(p => p.goods, p => p.goodsId)
                .ToPageListAsync(pageIndex, pageSize, totalCount);

            var list = new PageList<GoodsCollection>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        #endregion

        #region 收藏
        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public async Task<WebApiCallBack> ToAdd(int userId, int goodsId)
        {
            var jm = new WebApiCallBack();

            var goodsModel = await DbClient.Queryable<Goods>().Where(p => p.id == goodsId).FirstAsync();
            if (goodsModel == null)
            {
                jm.msg = "没有此商品";
                return jm;
            }
            var model = new GoodsCollection();
            model.userId = userId;
            model.goodsId = goodsId;
            model.createTime = DateTime.Now;
            model.goodsName = goodsModel.name;

            await DbClient.Insertable(model).ExecuteCommandAsync();

            jm.status = true;
            jm.msg = "收藏成功";

            return jm;

        }
        #endregion

    }
}
