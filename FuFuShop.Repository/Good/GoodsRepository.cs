using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;

namespace FuFuShop.Repository.Good
{
    /// <summary>
    /// 商品表 接口实现
    /// </summary>
    public class GoodsRepository : BaseRepository<Goods>, IGoodsRepository
    {
        public GoodsRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        /// <summary>
        /// 获取随机推荐数据
        /// </summary>
        /// <param name="number"></param>
        /// <param name="isRecommend"></param>
        /// <returns></returns>
        public async Task<List<Goods>> GetGoodsRecommendList(int number, bool isRecommend = false)
        {
            var list = new List<Goods>();

            if (isRecommend)
            {
                list = await DbClient.Queryable<Goods, Products>((good, pd) => new JoinQueryInfos(
                                JoinType.Left, good.id == pd.goodsId))
                        .Where((good, pd) => pd.isDefalut == true && pd.isDel == false && good.isRecommend == true && good.isDel == false && good.isMarketable == true)
                        .Select((good, pd) => new Goods
                        {
                            id = good.id,
                            bn = good.bn,
                            name = good.name,
                            brief = good.brief,
                            image = good.image,
                            images = good.images,
                            video = good.video,
                            productsDistributionType = good.productsDistributionType,
                            goodsCategoryId = good.goodsCategoryId,
                            goodsTypeId = good.goodsTypeId,
                            brandId = good.brandId,
                            isNomalVirtual = good.isNomalVirtual,
                            isMarketable = good.isMarketable,
                            unit = good.unit,
                            //intro = good.intro,
                            spesDesc = good.spesDesc,
                            parameters = good.parameters,
                            commentsCount = good.commentsCount,
                            viewCount = good.viewCount,
                            buyCount = good.buyCount,
                            uptime = good.uptime,
                            downtime = good.downtime,
                            sort = good.sort,
                            labelIds = good.labelIds,
                            newSpec = good.newSpec,
                            openSpec = good.openSpec,
                            createTime = good.createTime,
                            updateTime = good.updateTime,
                            isRecommend = good.isRecommend,
                            isHot = good.isHot,
                            isDel = good.isDel,
                            sn = pd.sn,
                            price = pd.price,
                            costprice = pd.costprice,
                            mktprice = pd.mktprice,
                            stock = pd.stock,
                            freezeStock = pd.freezeStock,
                            weight = pd.weight
                        })
                        .MergeTable()
                        .OrderBy(p => p.createTime, OrderByType.Desc)
                        .ToListAsync();
            }
            else
            {
                var ids = await DbClient.Queryable<Goods>().Where(p => p.isDel == false && p.isMarketable == true)
                    .Select(p => p.id).ToArrayAsync();
                var dbIds = new List<int>();
                if (ids.Any())
                {
                    for (int i = 0; i < number; i++)
                    {
                        var id = GetRandomNumber(ids);
                        dbIds.Add(id);
                    }
                    if (dbIds.Any())
                    {
                        list = await DbClient.Queryable<Goods, Products>((good, pd) => new JoinQueryInfos(
                                JoinType.Left, good.id == pd.goodsId))
                        .Where((good, pd) => pd.isDefalut == true && pd.isDel == false)
                        .Select((good, pd) => new Goods
                        {
                            id = good.id,
                            bn = good.bn,
                            name = good.name,
                            brief = good.brief,
                            image = good.image,
                            images = good.images,
                            video = good.video,
                            productsDistributionType = good.productsDistributionType,
                            goodsCategoryId = good.goodsCategoryId,
                            goodsTypeId = good.goodsTypeId,
                            brandId = good.brandId,
                            isNomalVirtual = good.isNomalVirtual,
                            isMarketable = good.isMarketable,
                            unit = good.unit,
                            //intro = good.intro,
                            spesDesc = good.spesDesc,
                            parameters = good.parameters,
                            commentsCount = good.commentsCount,
                            viewCount = good.viewCount,
                            buyCount = good.buyCount,
                            uptime = good.uptime,
                            downtime = good.downtime,
                            sort = good.sort,
                            labelIds = good.labelIds,
                            newSpec = good.newSpec,
                            openSpec = good.openSpec,
                            createTime = good.createTime,
                            updateTime = good.updateTime,
                            isRecommend = good.isRecommend,
                            isHot = good.isHot,
                            isDel = good.isDel,
                            sn = pd.sn,
                            price = pd.price,
                            costprice = pd.costprice,
                            mktprice = pd.mktprice,
                            stock = pd.stock,
                            freezeStock = pd.freezeStock,
                            weight = pd.weight
                        })
                        .MergeTable()
                        .Where(p => dbIds.Contains(p.id)).ToListAsync();
                    }
                }
            }
            return list;
        }

        // 随机抽取数组中的数据
        private static int GetRandomNumber(int[] a)
        {
            Random rnd = new Random();
            int index = rnd.Next(a.Length);
            return a[index];
        }


        #region 库存改变机制
        /// <summary>
        /// 库存改变机制。
        /// 库存机制：商品下单 总库存不变，冻结库存加1，
        /// 商品发货：冻结库存减1，总库存减1，
        /// 订单完成但未发货：总库存不变，冻结库存减1
        /// 商品退款&取消订单：总库存不变，冻结库存减1,
        /// 商品退货：总库存加1，冻结库存不变,
        /// 可销售库存：总库存-冻结库存
        /// </summary>
        /// <returns></returns>
        public WebApiCallBack ChangeStock(int productsId, string type = "order", int num = 0)
        {
            var res = new WebApiCallBack() { methodDescription = "库存改变机制" };

            if (productsId == 0)
            {
                res.msg = "货品ID不能为空";
                return res;
            }
            var productModel = DbClient.Queryable<Products>().InSingle(productsId);
            var bl = false;
            switch (type)
            {
                case "order": //下单
                              //更新订单购买量
                    DbClient.Updateable<Goods>().SetColumns(it => it.buyCount == it.buyCount + num).Where(p => p.id == productModel.goodsId).ExecuteCommand();
                    //判断是否有足够量去处理冻结库存变化
                    var insertNum = productModel.stock < productModel.freezeStock ? 0 : productModel.stock - productModel.freezeStock;
                    //更新记录。
                    bl = DbClient.Updateable<Products>()
                        .SetColumns(it => it.freezeStock == it.freezeStock + num)
                        .Where(p => p.id == productModel.id && insertNum >= num).ExecuteCommandHasChange();
                    break;
                case "send": //发货
                    bl = DbClient.Updateable<Products>()
                        .SetColumns(it => it.stock == it.stock - num)
                        .Where(p => p.freezeStock >= num && p.id == productModel.id).ExecuteCommandHasChange();
                    if (bl == true)
                    {
                        bl = DbClient.Updateable<Products>()
                            .SetColumns(it => it.freezeStock == it.freezeStock - num)
                            .Where(p => p.freezeStock >= num && p.id == productModel.id).ExecuteCommandHasChange();
                    }
                    else
                    {
                        res.status = true;
                        res.msg = "库存更新成功";
                        return res;
                    }
                    break;
                case "refund": //退款
                    bl = DbClient.Updateable<Products>()
                        .SetColumns(it => it.freezeStock == it.freezeStock - num)
                        .Where(p => p.id == productModel.id).ExecuteCommandHasChange();
                    break;
                case "return": //退货
                    bl = DbClient.Updateable<Products>()
                        .SetColumns(it => it.stock == it.stock + num)
                        .Where(p => p.id == productModel.id).ExecuteCommandHasChange();
                    break;
                case "cancel": //取消订单
                    bl = DbClient.Updateable<Products>()
                        .SetColumns(it => it.freezeStock == it.freezeStock - num)
                        .Where(p => p.id == productModel.id).ExecuteCommandHasChange();
                    break;
                case "complete": //完成订单
                    bl = DbClient.Updateable<Products>()
                        .SetColumns(it => it.freezeStock == it.freezeStock - num)
                        .Where(p => p.id == productModel.id).ExecuteCommandHasChange();
                    break;
                default:
                    bl = DbClient.Updateable<Products>()
                        .SetColumns(it => it.freezeStock == it.freezeStock + num)
                        .Where(p => p.id == productModel.id).ExecuteCommandHasChange();
                    break;
            }
            res.status = bl;
            res.msg = bl ? "库存更新成功" : "库存不足";

            return res;
        }
        #endregion
    }
}
