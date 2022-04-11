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

        public WebApiCallBack ChangeStock(int productsId, string type = "order", int num = 0)
        {
            throw new NotImplementedException();
        }
    }
}
