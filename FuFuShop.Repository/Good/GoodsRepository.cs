using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.Entities.Good;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;
using System.Linq.Expressions;

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
        /// 重写异步插入方法
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> InsertAsync(FMGoodsInsertModel entity)
        {
            var jm = new AdminUiCallBack();

            //基础验证
            if (!entity.products.Any())
            {
                jm.msg = "请设置至少一个货品信息！";
                return jm;
            }
            if (entity.products.Any())
            {
                var haveDefault = entity.products.Exists(p => p.isDefalut == true);
                if (haveDefault == false)
                {
                    jm.msg = "请设置某个货品为默认！";
                    return jm;
                }
            }
            if (string.IsNullOrEmpty(entity.goods.image))
            {
                jm.msg = "请上传封面图！";
                return jm;
            }
            if (string.IsNullOrEmpty(entity.goods.images))
            {
                jm.msg = "请上传图集！";
                return jm;
            }
            //检查数据
            foreach (var item in entity.products)
            {
                if (!item.sn.Contains("sn") && !item.sn.Contains("SN"))
                {
                    jm.msg = "请键入货品货号！货号为SN英文开头";
                    return jm;
                }
                else if (item.stock <= 0)
                {
                    jm.msg = "库存不能为0！";
                    return jm;
                }
                else if (item.price <= 0)
                {
                    jm.msg = "货品销售价格不能为0！";
                    return jm;
                }
                else if (item.mktprice <= 0 || item.mktprice < item.price)
                {
                    jm.msg = "货品市场价格不能为0并且不能小于销售价格！";
                    return jm;
                }
                else if (item.costprice <= 0 || item.costprice > item.price)
                {
                    jm.msg = "成本价格不能为0并且不能大于销售价格！";
                    return jm;
                }
                else if (item.levelOne < 0 || item.levelTwo < 0 || item.levelThree < 0)
                {
                    jm.msg = "返现金额不能小于0！";
                    return jm;
                }
            }
            //本社不允许重复
            var snsGroup = entity.products.GroupBy(x => x.sn).Where(x => x.Count() > 1).ToList();
            if (snsGroup.Count > 0)
            {
                jm.msg = "货品货号禁止存在重复";
                return jm;
            }

            //数据库不能存在已经有的编码
            var snsArr = entity.products.Select(p => p.sn).ToList();
            var blsn = await DbClient.Queryable<Products>().Where(p => snsArr.Contains(p.sn)).AnyAsync();
            if (blsn)
            {
                jm.msg = "系统中存在相同的货品货号，请重新生成货品货号。";
                return jm;
            }

            var bl = false;
            var goods = entity.goods;
            //goods.freezeStock = 0;
            goods.createTime = DateTime.Now;
            goods.commentsCount = 0;
            goods.buyCount = 0;
            goods.viewCount = 0;

            if (goods.openSpec == 1)
            {
                goods.newSpec = entity.goods.newSpec;
                goods.spesDesc = entity.goods.spesDesc;
                goods.parameters = entity.goods.parameters;
                goods.goodsTypeId = entity.goods.goodsTypeId;
            }
            else
            {
                goods.newSpec = "";
                goods.spesDesc = "";
                goods.parameters = "";
                goods.goodsTypeId = 0;
            }

            goods.isDel = false;
            goods.isMarketable = entity.goods.isMarketable;
            goods.isRecommend = entity.goods.isRecommend;
            goods.isHot = entity.goods.isHot;

            var id = await DbClient.Insertable(goods).ExecuteReturnIdentityAsync();
            bl = id > 0;
            if (id > 0)
            {


                if (goods.openSpec == 1)
                {
                    var products = new List<Products>();
                    entity.products.ForEach(p =>
                    {
                        var obj = new Products();
                        obj.goodsId = id;
                        obj.barcode = goods.bn;
                        obj.sn = p.sn;
                        obj.price = p.price;
                        obj.costprice = p.costprice;
                        obj.mktprice = p.mktprice;
                        obj.marketable = true;
                        obj.stock = p.stock;
                        obj.weight = p.weight;
                        obj.freezeStock = 0;
                        obj.spesDesc = p.spesDesc;
                        obj.isDefalut = p.isDefalut;
                        obj.isDel = false;
                        products.Add(obj);


                    });
                    //存入数据
                    var insertProducts = await DbClient.Insertable(products).ExecuteCommandAsync() > 0;
                    if (insertProducts)
                    {
                        //获取存入的数据，拿到序列，存储到分销表。因为sn可能前端会变更。所以只能用序列做唯一识别
                        var snArrys = products.Select(p => p.sn).ToList();
                        var results = await DbClient.Queryable<Products>().Where(p => snArrys.Contains(p.sn))
                            .ToListAsync();

                    }
                }
                else
                {
                    var oldObj = entity.products[0];
                    var obj = new Products();
                    obj.goodsId = id;
                    obj.barcode = goods.bn;
                    obj.sn = oldObj.sn;
                    obj.price = oldObj.price;
                    obj.costprice = oldObj.costprice;
                    obj.mktprice = oldObj.mktprice;
                    obj.marketable = true;
                    obj.stock = oldObj.stock;
                    obj.weight = oldObj.weight;
                    obj.freezeStock = 0;
                    obj.spesDesc = oldObj.spesDesc;
                    obj.isDefalut = true;
                    obj.isDel = false;
                    if (string.IsNullOrEmpty(obj.images) || !obj.images.Contains(".jpg"))
                    {
                        obj.images = goods.image;
                    }

                    var insertId = await DbClient.Insertable(obj).ExecuteReturnIdentityAsync();

                }

                if (!string.IsNullOrEmpty(entity.goodsCategoryExtendIds))
                {
                    var ids = CommonHelper.StringToIntArray(entity.goodsCategoryExtendIds);
                    if (ids.Any())
                    {
                        var extendIds = new List<GoodsCategoryExtend>();
                        foreach (var t in ids)
                        {
                            extendIds.Add(new GoodsCategoryExtend()
                            {
                                goodsCategroyId = t,
                                goodsId = id
                            });
                        }
                        bl = await DbClient.Insertable(extendIds).ExecuteCommandAsync() > 0;
                    }
                }
            }

            jm.data = entity;
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.CreateSuccess : GlobalConstVars.CreateFailure;


            return jm;
        }

        /// <summary>
        /// 重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> UpdateAsync(FMGoodsInsertModel entity)
        {
            var jm = new AdminUiCallBack();

            //基础验证
            if (!entity.products.Any())
            {
                jm.msg = "请设置至少一个货品信息！";
                return jm;
            }
            if (entity.products.Any())
            {
                var haveDefault = entity.products.Exists(p => p.isDefalut == true);
                if (haveDefault == false)
                {
                    jm.msg = "请设置一个货品为默认！";
                    return jm;
                }
            }
            if (string.IsNullOrEmpty(entity.goods.image))
            {
                jm.msg = "请上传封面图！";
                return jm;
            }
            if (string.IsNullOrEmpty(entity.goods.images))
            {
                jm.msg = "请上传图集！";
                return jm;
            }

            //检查数据
            foreach (var item in entity.products)
            {
                if (!item.sn.Contains("sn") && !item.sn.Contains("SN"))
                {
                    jm.msg = "请键入货品货号！货号为SN英文开头";
                    return jm;
                }
                else if (item.stock <= 0)
                {
                    jm.msg = "库存不能为0！";
                    return jm;
                }
                else if (item.price <= 0)
                {
                    jm.msg = "货品销售价格不能为0！";
                    return jm;
                }
                else if (item.mktprice <= 0 || item.mktprice < item.price)
                {
                    jm.msg = "货品市场价格不能为0并且不能小于销售价格！";
                    return jm;
                }
                else if (item.costprice <= 0 || item.costprice > item.price)
                {
                    jm.msg = "成本价格不能为0并且不能大于销售价格！";
                    return jm;
                }
                else if (item.levelOne < 0 || item.levelTwo < 0 || item.levelThree < 0)
                {
                    jm.msg = "返现金额不能小于0！";
                    return jm;
                }
            }

            var sns = entity.products.GroupBy(x => x.sn).Where(x => x.Count() > 1).ToList();
            if (sns.Count > 0)
            {
                jm.msg = "货品货号禁止存在重复";
                return jm;
            }

            //数据库不能存在已经有的编码
            var snsArr = entity.products.Select(p => p.sn).ToList();
            var blsn = await DbClient.Queryable<Products>().Where(p => snsArr.Contains(p.sn) && p.goodsId != entity.goods.id).AnyAsync();
            if (blsn)
            {
                jm.msg = "系统中存在相同的货品货号，请重新生成货品货号。";
                return jm;
            }


            var model = entity.goods;
            var oldModel = await DbClient.Queryable<Goods>().In(model.id).SingleAsync();
            if (oldModel == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            //事物处理过程开始
            //oldModel.id = model.id;
            oldModel.bn = model.bn;
            oldModel.name = model.name;
            oldModel.brief = model.brief;
            //oldModel.price = model.price;
            //oldModel.costprice = model.costprice;
            //oldModel.mktprice = model.mktprice;
            oldModel.images = model.images;
            oldModel.image = model.image;
            oldModel.video = model.video;
            oldModel.goodsCategoryId = model.goodsCategoryId;
            oldModel.brandId = model.brandId;
            oldModel.isNomalVirtual = model.isNomalVirtual;
            oldModel.isMarketable = model.isMarketable;
            //oldModel.stock = model.stock;
            //oldModel.freezeStock = model.freezeStock;
            //oldModel.weight = model.weight;
            oldModel.unit = model.unit;
            oldModel.intro = model.intro;
            oldModel.productsDistributionType = model.productsDistributionType;
            //oldModel.commentsCount = model.commentsCount;
            //oldModel.viewCount = model.viewCount;
            //oldModel.buyCount = model.buyCount;
            //oldModel.uptime = model.uptime;
            //oldModel.downtime = model.downtime;
            oldModel.sort = model.sort;
            //oldModel.labelIds = model.labelIds;
            //oldModel.createTime = model.createTime;
            oldModel.openSpec = model.openSpec;
            if (oldModel.openSpec == 1)
            {
                oldModel.newSpec = model.newSpec;
                oldModel.spesDesc = model.spesDesc;
                oldModel.parameters = model.parameters;
                oldModel.goodsTypeId = model.goodsTypeId;

                oldModel.goodsSkuIds = model.goodsSkuIds;
                oldModel.goodsParamsIds = model.goodsParamsIds;
                oldModel.parameters = model.parameters;

            }
            else
            {
                oldModel.newSpec = "";
                oldModel.spesDesc = "";
                oldModel.parameters = "";
                oldModel.goodsTypeId = 0;

                oldModel.goodsSkuIds = "";
                oldModel.goodsParamsIds = "";

            }


            oldModel.updateTime = DateTime.Now;
            oldModel.isRecommend = model.isRecommend;
            oldModel.isHot = model.isHot;
            oldModel.isDel = model.isDel;

            var bl = await DbClient.Updateable(oldModel).ExecuteCommandHasChangeAsync();
            if (bl)
            {
                if (!string.IsNullOrEmpty(entity.goodsCategoryExtendIds))
                {
                    bl = await DbClient.Deleteable<GoodsCategoryExtend>().Where(p => p.goodsId == model.id).ExecuteCommandHasChangeAsync();
                    var ids = CommonHelper.StringToIntArray(entity.goodsCategoryExtendIds);
                    if (ids.Any())
                    {
                        var extendIds = new List<GoodsCategoryExtend>();
                        foreach (var t in ids)
                        {
                            extendIds.Add(new GoodsCategoryExtend()
                            {
                                goodsCategroyId = t,
                                goodsId = model.id
                            });
                        }
                        bl = await DbClient.Insertable(extendIds).ExecuteCommandAsync() > 0;
                    }
                }

                //商品信息处理
                var products = await DbClient.Queryable<Products>().Where(p => p.goodsId == model.id && p.isDel == false).ToListAsync();
                if (oldModel.openSpec == 1)
                {
                    var oldPostProducts = entity.products.Where(p => p.id > 0).ToList();
                    if (oldPostProducts.Any())
                    {
                        var oldPostProductsIds = oldPostProducts.Select(p => p.id).ToList();
                        //需要删除的数据
                        var deletes = products.Where(p => !oldPostProductsIds.Contains(p.id)).ToList();
                        if (deletes.Any())
                        {
                            var deleteIds = deletes.Select(p => p.id).ToList();
                            bl = await DbClient.Updateable<Products>().SetColumns(p => p.isDel == true).Where(p => deleteIds.Contains(p.id)).ExecuteCommandHasChangeAsync();
                        }
                    }
                    else
                    {
                        var pIds = products.Select(p => p.id).ToList();
                        bl = await DbClient.Updateable<Products>().SetColumns(p => p.isDel == true).Where(p => pIds.Contains(p.id)).ExecuteCommandHasChangeAsync();
                    }

                    //新数据
                    var newPostProducts = entity.products.Where(p => p.id == 0).ToList();
                    var newProducts = new List<Products>();
                    if (newPostProducts.Any())
                    {
                        newPostProducts.ForEach(p =>
                        {
                            var obj = new Products();
                            obj.goodsId = model.id;
                            obj.barcode = model.bn;
                            obj.sn = p.sn;
                            obj.price = p.price;
                            obj.costprice = p.costprice;
                            obj.mktprice = p.mktprice;
                            obj.marketable = true;
                            obj.stock = p.stock;
                            obj.weight = p.weight;
                            obj.freezeStock = 0;
                            obj.spesDesc = p.spesDesc;
                            obj.isDefalut = p.isDefalut;
                            obj.images = p.images;
                            if (string.IsNullOrEmpty(p.images) || !p.images.Contains(".jpg"))
                            {
                                obj.images = oldModel.image;
                            }
                            obj.isDel = false;
                            newProducts.Add(obj);


                        });
                        var insertProducts = await DbClient.Insertable(newProducts).ExecuteCommandAsync() > 0;
                        if (insertProducts)
                        {
                            //获取存入的数据，拿到序列，存储到分销表。因为sn可能前端会变更。所以只能用序列做唯一识别
                            var snArrys = newProducts.Select(p => p.sn).ToList();
                            var results = await DbClient.Queryable<Products>().Where(p => snArrys.Contains(p.sn))
                                .ToListAsync();

                        }
                    }
                }
                else
                {
                    await DbClient.Updateable<Products>().SetColumns(p => p.isDel == true).Where(p => p.goodsId == model.id).ExecuteCommandHasChangeAsync();
                    var newObj = entity.products.FirstOrDefault();

                    if (newObj is { id: > 0 })
                    {
                        var obj = products.Find(p => p.id == newObj.id);
                        //obj.barcode = model.bn;
                        if (obj != null)
                        {
                            obj.sn = newObj.sn;
                            obj.price = newObj.price;
                            obj.costprice = newObj.costprice;
                            obj.mktprice = newObj.mktprice;
                            obj.marketable = true;
                            obj.stock = newObj.stock;
                            obj.weight = newObj.weight;
                            obj.images = newObj.images;
                            obj.isDefalut = true;
                            obj.isDel = false;
                            if (string.IsNullOrEmpty(newObj.images) || !newObj.images.Contains(".jpg"))
                            {
                                obj.images = oldModel.image;
                            }

                            bl = await DbClient.Updateable<Products>(obj).ExecuteCommandHasChangeAsync();
                        }

                    }
                    else
                    {
                        var obj = new Products();
                        obj.goodsId = model.id;
                        obj.barcode = model.bn;
                        obj.sn = newObj.sn;
                        obj.price = newObj.price;
                        obj.costprice = newObj.costprice;
                        obj.mktprice = newObj.mktprice;
                        obj.images = newObj.images;
                        obj.marketable = true;
                        obj.stock = newObj.stock;
                        obj.weight = newObj.weight;
                        obj.freezeStock = 0;
                        obj.spesDesc = "";
                        obj.isDefalut = true;
                        obj.isDel = false;
                        if (string.IsNullOrEmpty(newObj.images) || !newObj.images.Contains(".jpg"))
                        {
                            obj.images = oldModel.image;
                        }
                        var insertId = await DbClient.Insertable<Products>(obj).ExecuteReturnIdentityAsync();

                    }
                }
            }
            //事物处理过程结束
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;


            return jm;
        }



        #region 重写删除指定ID集合的数据(批量删除)
        /// <summary>
        /// 重写删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> DeleteByIdsAsync(int[] ids)
        {
            var jm = new AdminUiCallBack();

            var bl = await DbClient.Updateable<Goods>().SetColumns(p => p.isDel == true).Where(p => ids.Contains(p.id)).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.DeleteSuccess : GlobalConstVars.DeleteFailure;

            return jm;
        }

        #endregion

        #region 获取商品重量

        /// <summary>
        /// 获取商品重量
        /// </summary>
        /// <param name="productsId"></param>
        /// <returns></returns>
        public async Task<decimal> GetWeight(int productsId)
        {
            decimal weight = 0;
            //var productsModel =await DbClient.Queryable<Products, Goods>((products, goods) => new object[]
            //    {
            //        JoinType.Left, products.goodsId == goods.id
            //    }).Where((products, goods) => products.id == productsId)
            //    .Select((products, goods) => new { goodsWeight = goods.weight, productsWeight = products.weight }).FirstAsync();
            //if (productsModel != null)
            //{
            //    weight = productsModel.productsWeight > 0 ? productsModel.productsWeight : productsModel.goodsWeight > 0 ? productsModel.goodsWeight : 0;
            //}

            var productsModel = await DbClient.Queryable<Products>().FirstAsync(p => p.id == productsId);
            if (productsModel != null)
            {
                weight = productsModel.weight > 0 ? productsModel.weight : 0;
            }
            return weight > 0 ? weight : 0;
        }
        #endregion

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

        #region 获取随机推荐商品数据

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
        static int GetRandomNumber(int[] a)
        {
            Random rnd = new Random();
            int index = rnd.Next(a.Length);
            return a[index];
        }

        #endregion

        #region 获取数据总数
        /// <summary>
        ///     获取数据总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public new async Task<int> GetCountAsync(Expression<Func<Goods, bool>> predicate, bool blUseNoLock = false)
        {
            var count = 0;
            if (blUseNoLock)
            {
                count = await DbClient.Queryable<Goods, Products>((good, pd) => new JoinQueryInfos(
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
                .With(SqlWith.NoLock)
                    .MergeTable()
                    .CountAsync(predicate);
            }
            else
            {
                count = await DbClient.Queryable<Goods, Products>((good, pd) => new JoinQueryInfos(
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
                    .CountAsync(predicate);
            }
            return count;
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
        public new async Task<IPageList<Goods>> QueryPageAsync(Expression<Func<Goods, bool>> predicate,
            Expression<Func<Goods, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false)
        {
            RefAsync<int> totalCount = 0;
            List<Goods> page;
            if (blUseNoLock)
            {
                page = await DbClient.Queryable<Goods, Products>((good, pd) => new JoinQueryInfos(
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
                .With(SqlWith.NoLock)
                    .MergeTable()
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .WhereIF(predicate != null, predicate)
                .ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            else
            {
                page = await DbClient.Queryable<Goods, Products>((good, pd) => new JoinQueryInfos(
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
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .WhereIF(predicate != null, predicate)
                .ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            var list = new PageList<Goods>(page, pageIndex, pageSize, totalCount);
            return list;
        }
        #endregion

        #region 根据条件查询一定数量数据
        /// <summary>
        ///     根据条件查询一定数量数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="take">获取数量</param>
        /// <param name="orderByPredicate">排序字段</param>
        /// <param name="orderByType">排序顺序</param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public new async Task<List<Goods>> QueryListByClauseAsync(Expression<Func<Goods, bool>> predicate, int take,
            Expression<Func<Goods, object>> orderByPredicate, OrderByType orderByType, bool blUseNoLock = false)
        {
            //return blUseNoLock
            //    ? await DbBaseClient.Queryable<T>().OrderByIF(orderByPredicate != null, orderByPredicate, orderByType)
            //        .WhereIF(predicate != null, predicate).Take(take).With(SqlWith.NoLock).ToListAsync()
            //    : await DbBaseClient.Queryable<T>().OrderByIF(orderByPredicate != null, orderByPredicate, orderByType)
            //        .WhereIF(predicate != null, predicate).Take(take).ToListAsync();

            List<Goods> page;
            if (blUseNoLock)
            {
                page = await DbClient.Queryable<Goods, Products>((good, pd) => new JoinQueryInfos(
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
                .With(SqlWith.NoLock)
                    .MergeTable()
                .OrderByIF(orderByPredicate != null, orderByPredicate, orderByType)
                .WhereIF(predicate != null, predicate)
                    .Take(take).ToListAsync();
            }
            else
            {
                page = await DbClient.Queryable<Goods, Products>((good, pd) => new JoinQueryInfos(
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
                    .OrderByIF(orderByPredicate != null, orderByPredicate, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .Take(take).ToListAsync();
            }
            return page;

        }
        #endregion

        #region 重写根据条件查询数据
        /// <summary>
        ///     重写根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy"></param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public new async Task<List<Goods>> QueryListByClauseAsync(Expression<Func<Goods, bool>> predicate, string orderBy = "",
            bool blUseNoLock = false)
        {
            List<Goods> page;
            if (blUseNoLock)
            {
                page = await DbClient.Queryable<Goods, Products>((good, pd) => new JoinQueryInfos(
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
                .With(SqlWith.NoLock)
                    .MergeTable()
                .OrderByIF(!string.IsNullOrEmpty(orderBy), orderBy)
                .WhereIF(predicate != null, predicate)
                    .ToListAsync();
            }
            else
            {
                page = await DbClient.Queryable<Goods, Products>((good, pd) => new JoinQueryInfos(
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
                    .OrderByIF(!string.IsNullOrEmpty(orderBy), orderBy)
                    .WhereIF(predicate != null, predicate)
                    .ToListAsync();
            }
            return page;

        }
        #endregion

        #region 根据条件查询分页数据
        /// <summary>
        ///     根据条件查询分页数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public new async Task<IPageList<Goods>> QueryPageAsync(Expression<Func<Goods, bool>> predicate, string orderBy = "",
            int pageIndex = 1, int pageSize = 20, bool blUseNoLock = false)
        {
            RefAsync<int> totalCount = 0;
            List<Goods> page;
            if (blUseNoLock)
            {
                page = await DbClient.Queryable<Goods, Products>((good, pd) => new JoinQueryInfos(
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
                    .With(SqlWith.NoLock)
                    .MergeTable()
                    .Where(predicate)
                    .OrderBy(orderBy)
                    .ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            else
            {
                page = await DbClient.Queryable<Goods, Products>((good, pd) => new JoinQueryInfos(
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
                    .Where(predicate)
                    .OrderBy(orderBy)
                    .ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            var list = new PageList<Goods>(page, pageIndex, pageSize, totalCount);
            return list;


        }


        #endregion




        #region 获取下拉商品数据
        /// <summary>
        ///     获取下拉商品数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<EnumEntity>> QueryEnumEntityAsync()
        {
            List<EnumEntity> list = await DbClient.Queryable<Goods>()
                .Where(p => p.isDel == false)
                .Select(p => new EnumEntity
                {
                    value = p.id,
                    title = p.name,
                    description = p.image,
                })
                .With(SqlWith.NoLock).OrderBy(p => p.value, OrderByType.Desc).ToListAsync();
            return list;

        }

        public Task<IPageList<Goods>> QueryAgentGoodsPageAsync(Expression<Func<Goods, bool>> predicate, string orderBy = "", int pageIndex = 1, int pageSize = 20, bool blUseNoLock = false)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}