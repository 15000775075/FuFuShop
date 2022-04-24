using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Common.Extensions;
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository.Good
{
    /// <summary>
    /// 货品表 接口实现
    /// </summary>
    public class ProductsRepository : BaseRepository<Products>, IProductsRepository
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextUser _user;
        private readonly IStockRepository _stockRepository;

        public ProductsRepository(
            IUnitOfWork unitOfWork,
            IHttpContextUser user,
            IStockRepository stockRepository) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _user = user;
            _stockRepository = stockRepository;
        }

        #region 判断货品上下架状态

        /// <summary>
        /// 判断货品上下架状态
        /// </summary>
        /// <param name="productsId">货品序列</param>
        /// <returns></returns>
        public async Task<bool> GetShelfStatus(int productsId)
        {
            var data = await DbClient.Queryable<Products, Goods>((products, goods) => new object[]
                  {
                    JoinType.Inner, products.goodsId == goods.id
                  })
                .Where((products, goods) => products.id == productsId)
                .Select((products, goods) => new
                {
                    productsId = products.id,
                    goods.isMarketable
                }).FirstAsync();
            return data != null && data.isMarketable == true;
        }
        #endregion

        #region 获取库存报警数量
        /// <summary>
        /// 获取库存报警数量
        /// </summary>
        /// <param name="goodsStocksWarn"></param>
        /// <returns></returns>
        public async Task<int> GoodsStaticsTotalWarn(int goodsStocksWarn)
        {
            var sql = @"SELECT  COUNT(*) AS number
                        FROM    ( SELECT    t.goodsId
                                  FROM      ( SELECT    goodsId ,
                                                        ( CASE WHEN stock < freezeStock THEN 0
                                                               ELSE stock - freezeStock
                                                          END ) AS number
                                              FROM     FuFuShop_Products
                                            ) t
                                  WHERE     t.number < " + goodsStocksWarn + @"
                                  GROUP BY  t.goodsId
                                ) d";

            var dt = await DbClient.Ado.GetDataTableAsync(sql);
            var number = dt.Rows[0][0].ObjectToInt(0);
            return number;
        }

        #endregion

        #region 获取关联商品的货品列表数据
        /// <summary>
        ///     获取关联商品的货品列表数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public async Task<IPageList<Products>> QueryDetailPageAsync(Expression<Func<Products, bool>> predicate,
            Expression<Func<Products, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false)
        {
            RefAsync<int> totalCount = 0;
            List<Products> page;
            if (blUseNoLock)
            {
                page = await DbClient.Queryable<Products, Goods>((p, good) => new JoinQueryInfos(
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
                    .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            else
            {
                page = await DbClient.Queryable<Products, Goods>((p, good) => new JoinQueryInfos(
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
                    })
                    .MergeTable()
                    .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            var list = new PageList<Products>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        #endregion



        #region 获取货品数据
        /// <summary>
        /// 获取货品数据
        /// </summary>
        /// <param name="goodId"></param>
        /// <returns></returns>
        public async Task<List<Products>> GetProducts(int goodId = 0)
        {
            var list = await DbClient.Queryable<Products, Goods>((p, good) => new JoinQueryInfos(
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
                   .OrderBy(p => p.id, OrderByType.Desc)
                   .WhereIF(goodId > 0, p => p.goodsId == goodId)
                   .Where(p => p.isDel == false)
                   .ToListAsync();

            return list;
        }

        #endregion


        #region 修改单个货品库存并记入库存管理日志内

        /// <summary>
        /// 修改单个货品库存并记入库存管理日志内
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="stockNumber"></param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> EditStock(int productId, int stockNumber)
        {
            var jm = new AdminUiCallBack();

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
                })
                .MergeTable()
                .Where(p => p.id == productId)
                .FirstAsync();
            if (product == null)
            {
                jm.msg = "货品数据查询失败";
                return jm;
            }


            var nums = stockNumber - product.stock;
            var msg = string.Empty;
            if (nums == 0)
            {
                jm.code = 0;
                jm.msg = "库存未修改";
                return jm;
            }
            else if (nums < 0)
            {
                jm.code = 0;
                msg = "库存盘点：库存减少" + Math.Abs(nums);
            }
            else
            {
                msg = "库存盘点：库存增加" + nums;
            }

            var stockModel = new Stock();
            stockModel.id = await _stockRepository.CreateCode(GlobalEnumVars.StockType.CheckGoods.ToString());
            stockModel.memo = msg;
            stockModel.type = (int)GlobalEnumVars.StockType.CheckGoods;
            stockModel.manager = _user.ID;
            stockModel.createTime = DateTime.Now;

            var stockLogModel = new StockLog();
            stockLogModel.stockId = stockModel.id;
            stockLogModel.productId = product.id;
            stockLogModel.goodsId = product.goodsId;
            stockLogModel.nums = nums;
            stockLogModel.goodsName = product.name;
            stockLogModel.sn = product.sn;
            stockLogModel.bn = product.bn;
            stockLogModel.spesDesc = product.spesDesc;

            try
            {
                _unitOfWork.BeginTran();

                await DbClient.Updateable<Products>().SetColumns(p => new Products() { stock = stockNumber }).Where(p => p.id == product.id).ExecuteCommandAsync();

                await DbClient.Insertable(stockModel).ExecuteCommandAsync();
                await DbClient.Insertable(stockLogModel).ExecuteCommandAsync();

                jm.code = 0;
                jm.msg = "库存修改成功";

                _unitOfWork.CommitTran();
            }
            catch (Exception e)
            {
                jm.code = 1;
                jm.msg = "库存修改异常";
                jm.otherData = e;
                _unitOfWork.RollbackTran();
            }
            return jm;
        }


        #endregion



    }
}
