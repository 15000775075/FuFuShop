/***********************************************************************
 *            Project: 
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www..net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/1/31 21:45:10
 *        Description: 暂无
 ***********************************************************************/

using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 库存操作表 接口实现
    /// </summary>
    public class StockRepository : BaseRepository<Stock>, IStockRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public StockRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region 实现重写增删改查操作==========================================================

        /// <summary>
        /// 重写异步插入方法
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> InsertAsync(FMCreateStock entity)
        {
            var jm = new AdminUiCallBack();

            if (entity.items == null || !entity.items.Any())
            {
                jm.msg = "至少选择一个货品哦";
                return jm;
            }

            bool isRepeat = entity.items.GroupBy(i => i.productId).Any(g => g.Count() > 1);
            if (isRepeat)
            {
                jm.msg = "请勿提交相同货品";
                return jm;
            }

            if (entity.items.Any(p => p.nums <= 0))
            {
                jm.msg = "入库出库的货品数量不能为0";
                return jm;
            }

            var stockModel = new Stock();
            if (entity.model.type == (int)GlobalEnumVars.StockType.In)
            {
                stockModel.id = await CreateCode(GlobalEnumVars.StockType.In.ToString());
            }
            else if (entity.model.type == (int)GlobalEnumVars.StockType.Out)
            {
                stockModel.id = await CreateCode(GlobalEnumVars.StockType.Out.ToString());
            }
            else
            {
                jm.msg = "单据类型错误";
                return jm;
            }

            stockModel.memo = entity.model.memo;
            stockModel.createTime = DateTime.Now;
            stockModel.manager = entity.model.manager;
            stockModel.type = entity.model.type;

            var logs = new List<StockLog>();
            var products = new List<Products>();
            var index = 0;
            foreach (var item in entity.items)
            {
                index++;
                //判断此货品是否存在
                var product = await DbClient.Queryable<Products, Goods>((p, good) => new JoinQueryInfos(
                        JoinType.Left, p.goodsId == good.id))
                    .Select((p, good) => new Products
                    {
                        id = p.id,
                        goodsId = p.goodsId,
                        barcode = p.barcode,
                        sn = p.sn,
                        price = p.price,
                        costprice = p.costprice,
                        mktprice = p.mktprice,
                        marketable = p.marketable,
                        weight = p.weight,
                        stock = p.stock,
                        freezeStock = p.freezeStock,
                        spesDesc = p.spesDesc,
                        isDefalut = p.isDefalut,
                        images = p.images,
                        isDel = p.isDel,
                        name = good.name,
                        bn = good.bn,
                        isMarketable = good.isMarketable,
                        unit = good.unit
                    }).With(SqlWith.NoLock)
                    .MergeTable()
                    .Where(p => p.id == item.productId).FirstAsync();
                if (product != null && item.nums > 0)
                {
                    var stock = 0;
                    if (entity.model.type == (int)GlobalEnumVars.StockType.In)
                    {
                        stock = product.stock + item.nums;
                    }
                    else if (entity.model.type == (int)GlobalEnumVars.StockType.Out)
                    {
                        stock = product.stock - item.nums;
                        if (stock < 0)
                        {
                            jm.msg = $"第{index}个货品最大出库数量为：" + product.stock;
                            return jm;
                        }
                    }
                    product.stock = stock;
                    products.Add(product);

                    var log = new StockLog
                    {
                        stockId = stockModel.id,
                        productId = product.id,
                        goodsId = product.goodsId,
                        nums = item.nums,
                        sn = product.sn,
                        bn = product.barcode,
                        goodsName = product.name,
                        spesDesc = product.spesDesc
                    };
                    logs.Add(log);
                }
                else
                {
                    jm.msg = $"请检查第{index}个货品或数量是否正确";
                    return jm;
                }
            }

            try
            {
                _unitOfWork.BeginTran();
                var bl = await DbClient.Insertable(stockModel).ExecuteCommandAsync() > 0;
                if (products.Any())
                {
                    await DbClient.Updateable(products).ExecuteCommandAsync();
                }
                if (logs.Any())
                {
                    await DbClient.Insertable(logs).ExecuteCommandAsync();
                }
                jm.code = bl ? 0 : 1;
                jm.msg = bl ? GlobalConstVars.CreateSuccess : GlobalConstVars.CreateFailure;
                _unitOfWork.CommitTran();
            }
            catch (Exception e)
            {
                _unitOfWork.RollbackTran();
                jm.code = 1;
                jm.msg = "处理异常";
                jm.data = e;
            }
            return jm;
        }

        /// <summary>
        /// 重写异步更新方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> UpdateAsync(Stock entity)
        {
            var jm = new AdminUiCallBack();

            var oldModel = await DbClient.Queryable<Stock>().In(entity.id).SingleAsync();
            if (oldModel == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            //事物处理过程开始
            oldModel.id = entity.id;
            oldModel.type = entity.type;
            oldModel.manager = entity.manager;
            oldModel.memo = entity.memo;
            oldModel.createTime = entity.createTime;

            //事物处理过程结束
            var bl = await DbClient.Updateable(oldModel).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;

            return jm;
        }

        /// <summary>
        /// 重写异步更新方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> UpdateAsync(List<Stock> entity)
        {
            var jm = new AdminUiCallBack();

            var bl = await DbClient.Updateable(entity).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;

            return jm;
        }

        /// <summary>
        /// 重写删除指定ID的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> DeleteByIdAsync(object id)
        {
            var jm = new AdminUiCallBack();

            var bl = await DbClient.Deleteable<Stock>(id).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.DeleteSuccess : GlobalConstVars.DeleteFailure;

            return jm;
        }

        /// <summary>
        /// 重写删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> DeleteByIdsAsync(int[] ids)
        {
            var jm = new AdminUiCallBack();

            var bl = await DbClient.Deleteable<Stock>().In(ids).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.DeleteSuccess : GlobalConstVars.DeleteFailure;

            return jm;
        }

        #endregion


        #region 重写根据条件查询分页数据
        /// <summary>
        ///     重写根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public new async Task<IPageList<Stock>> QueryPageAsync(Expression<Func<Stock, bool>> predicate,
            Expression<Func<Stock, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false)
        {
            RefAsync<int> totalCount = 0;
            List<Stock> page;
            if (blUseNoLock)
            {
                page = await DbClient.Queryable<Stock, SysUser>((p, sUser) => new JoinQueryInfos(
                        JoinType.Left, p.manager == sUser.id))
                .Select((p, sUser) => new Stock
                {
                    id = p.id,
                    type = p.type,
                    manager = p.manager,
                    memo = p.memo,
                    createTime = p.createTime,
                    managerName = sUser.nickName
                })
                .With(SqlWith.NoLock)
                .MergeTable()
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .WhereIF(predicate != null, predicate)
                .ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            else
            {
                page = await DbClient.Queryable<Stock, SysUser>((p, sUser) => new JoinQueryInfos(JoinType.Left, p.manager == sUser.id))
                    .Select((p, sUser) => new Stock
                    {
                        id = p.id,
                        type = p.type,
                        manager = p.manager,
                        memo = p.memo,
                        createTime = p.createTime,
                        managerName = sUser.nickName
                    })
                    .MergeTable()
                    .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            var list = new PageList<Stock>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        #endregion


        /// <summary>
        /// 生成唯一单号
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<string> CreateCode(string type)
        {
            Random rand = new Random();
            while (true)
            {
                var str = string.Empty;
                if (type == GlobalEnumVars.StockType.In.ToString())
                {
                    str = "sI";
                }
                else if (type == GlobalEnumVars.StockType.Out.ToString())
                {
                    str = "sO";
                }
                else
                {
                    str = "sU";
                }

                str += CommonHelper.Msectime() + rand.Next(0, 9);
                var bl = await DbClient.Queryable<Stock>().AnyAsync(p => p.id == str);
                if (bl == false)
                {
                    return str;
                }
            }
        }


    }
}
