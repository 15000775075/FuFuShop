using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.Good;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Services.Good
{
    /// <summary>
    /// 商品表 接口实现
    /// </summary>
    public class GoodsServices : BaseServices<Goods>, IGoodsServices
    {
        private readonly IGoodsRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductsServices _productsServices;
        private readonly ILabelServices _labelServices;
        private readonly IGoodsCollectionServices _goodsCollectionServices;
        private readonly IBrandServices _brandServices;
        private readonly IOrderItemServices _orderItemServices;

        public GoodsServices(IUnitOfWork unitOfWork, IGoodsRepository dal
            , IProductsServices productsServices
            , ILabelServices labelServices
            , IGoodsCollectionServices goodsCollectionServices
            , IBrandServices brandServices
            , IOrderItemServices orderItemServices
        )
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
            _productsServices = productsServices;
            _labelServices = labelServices;
            _goodsCollectionServices = goodsCollectionServices;
            _brandServices = brandServices;
            _orderItemServices = orderItemServices;
        }


        /// <summary>
        /// 重写异步插入方法
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> InsertAsync(FMGoodsInsertModel entity)
        {
            return await _dal.InsertAsync(entity);
        }

        /// <summary>
        /// 重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> UpdateAsync(FMGoodsInsertModel entity)
        {
            return await _dal.UpdateAsync(entity);
        }

        /// <summary>
        /// 重写删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> DeleteByIdsAsync(int[] ids)
        {
            return await _dal.DeleteByIdsAsync(ids);
        }


        #region 批量修改价格==========================================
        /// <summary>
        /// 批量修改价格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> DoBatchModifyPrice(FmBatchModifyPrice entity)
        {
            var jm = new AdminUiCallBack();

            var bl = false;
            //获取商品信息
            var goods = await base.BaseDal.QueryListByClauseAsync(p => entity.ids.Contains(p.id));
            if (!goods.Any())
            {
                jm.msg = GlobalConstVars.DataisNo;
                return jm;
            }
            var goodIds = goods.Select(p => p.id).ToList();
            //获取货品信息
            var products = await _productsServices.QueryListByClauseAsync(p => goodIds.Contains(p.goodsId));
            var productIds = products.Select(p => p.id).ToList();

            var priceValue = entity.priceValue;

            switch (entity.modifyType)
            {
                case "=":
                    if (entity.priceType == GlobalEnumVars.PriceType.price.ToString())
                    {
                        bl = await _productsServices.UpdateAsync(p => new Products() { price = priceValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    }
                    else if (entity.priceType == GlobalEnumVars.PriceType.costprice.ToString())
                    {
                        bl = await _productsServices.UpdateAsync(p => new Products() { costprice = priceValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    }
                    else if (entity.priceType == GlobalEnumVars.PriceType.mktprice.ToString())
                    {
                        bl = await _productsServices.UpdateAsync(p => new Products() { mktprice = priceValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    }
                    else if (entity.priceType.Contains("grade_price_"))
                    {
                        var gradeArr = entity.priceType.Split("_");
                        var goodsGradeId = Convert.ToInt16(gradeArr[2]);
                        if (goodsGradeId > 0)
                        {
                        }
                    }
                    break;
                case "-":
                    if (entity.priceType == GlobalEnumVars.PriceType.price.ToString())
                    {
                        bl = await _productsServices.UpdateAsync(p => new Products() { price = p.price - priceValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    }
                    else if (entity.priceType == GlobalEnumVars.PriceType.costprice.ToString())
                    {
                        bl = await _productsServices.UpdateAsync(p => new Products() { costprice = p.costprice - priceValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    }
                    else if (entity.priceType == GlobalEnumVars.PriceType.mktprice.ToString())
                    {
                        bl = await _productsServices.UpdateAsync(p => new Products() { mktprice = p.mktprice - priceValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    }
                    else if (entity.priceType.Contains("grade_price_"))
                    {
                        var gradeArr = entity.priceType.Split("_");
                        var goodsGradeId = Convert.ToInt16(gradeArr[2]);
                        if (goodsGradeId > 0)
                        {
                        }
                    }
                    break;
                case "+":
                    if (entity.priceType == GlobalEnumVars.PriceType.price.ToString())
                    {
                        bl = await _productsServices.UpdateAsync(p => new Products() { price = p.price + priceValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    }
                    else if (entity.priceType == GlobalEnumVars.PriceType.costprice.ToString())
                    {
                        bl = await _productsServices.UpdateAsync(p => new Products() { costprice = p.costprice + priceValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    }
                    else if (entity.priceType == GlobalEnumVars.PriceType.mktprice.ToString())
                    {
                        bl = await _productsServices.UpdateAsync(p => new Products() { mktprice = p.mktprice + priceValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    }
                    else if (entity.priceType.Contains("grade_price_"))
                    {
                        var gradeArr = entity.priceType.Split("_");
                        var goodsGradeId = Convert.ToInt16(gradeArr[2]);
                        if (goodsGradeId > 0)
                        {
                        }
                    }
                    break;
                case "*":
                    if (entity.priceType == GlobalEnumVars.PriceType.price.ToString())
                    {
                        bl = await _productsServices.UpdateAsync(p => new Products() { price = p.price * priceValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    }
                    else if (entity.priceType == GlobalEnumVars.PriceType.costprice.ToString())
                    {
                        bl = await _productsServices.UpdateAsync(p => new Products() { costprice = p.costprice * priceValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    }
                    else if (entity.priceType == GlobalEnumVars.PriceType.mktprice.ToString())
                    {
                        bl = await _productsServices.UpdateAsync(p => new Products() { mktprice = p.mktprice * priceValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    }
                    else if (entity.priceType.Contains("grade_price_"))
                    {
                        var gradeArr = entity.priceType.Split("_");
                        var goodsGradeId = Convert.ToInt16(gradeArr[2]);
                        if (goodsGradeId > 0)
                        {
                        }
                    }
                    break;
            }
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? "价格修改成功" : "价格修改失败";

            return jm;
        }

        #endregion

        #region 批量修改库存==========================================
        /// <summary>
        /// 批量修改价格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> DoBatchModifyStock(FmBatchModifyStock entity)
        {
            var jm = new AdminUiCallBack();

            var bl = false;
            //获取商品信息
            var goods = await base.BaseDal.QueryListByClauseAsync(p => entity.ids.Contains(p.id));
            if (!goods.Any())
            {
                jm.msg = GlobalConstVars.DataisNo;
                return jm;
            }
            var goodIds = goods.Select(p => p.id).ToList();

            var modifyValue = entity.modifyValue;

            switch (entity.modifyType)
            {
                case "=":
                    bl = await _productsServices.UpdateAsync(p => new Products() { stock = modifyValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    break;
                case "-":
                    bl = await _productsServices.UpdateAsync(p => new Products() { stock = p.stock - modifyValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    break;
                case "+":
                    bl = await _productsServices.UpdateAsync(p => new Products() { stock = p.stock + modifyValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    break;
                case "*":
                    bl = await _productsServices.UpdateAsync(p => new Products() { stock = p.stock * modifyValue }, p => goodIds.Contains(p.goodsId) && p.isDel == false);
                    break;
            }
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? "库存修改成功" : "库存修改失败";

            return jm;
        }

        #endregion

        #region 批量上架==========================================

        /// <summary>
        /// 批量上架
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> DoBatchMarketableUp(int[] ids)
        {
            var jm = new AdminUiCallBack();

            var bl = await _dal.UpdateAsync(p => new Goods() { isMarketable = true }, p => ids.Contains(p.id));
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? "上架成功" : "上架失败";

            return jm;
        }
        #endregion

        #region 批量下架==========================================

        /// <summary>
        /// 批量下架
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> DoBatchMarketableDown(int[] ids)
        {
            var jm = new AdminUiCallBack();

            var bl = await base.BaseDal.UpdateAsync(p => new Goods() { isMarketable = false }, p => ids.Contains(p.id));
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? "下架成功" : "下架失败";

            return jm;
        }
        #endregion

        #region 设置标签==========================================

        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> DoSetLabel(FmSetLabel entity)
        {
            var jm = new AdminUiCallBack();


            var names = entity.labels.Select(p => p.text).ToList();
            //获取已经存在数据库数据
            var olds = await _labelServices.QueryListByClauseAsync(p => names.Contains(p.name));
            if (olds.Any())
            {
                var oldNames = olds.Select(p => p.name).ToList();
                //获取未插入数据库数据
                var newNames = entity.labels.Where(p => !oldNames.Contains(p.text)).ToList();
                if (newNames.Any())
                {
                    var labels = new List<Label>();
                    newNames.ForEach(p =>
                    {
                        labels.Add(new Label()
                        {
                            name = p.text,
                            style = p.style
                        });
                    });
                    await _labelServices.InsertAsync(labels);
                }
            }
            else
            {
                var labels = new List<Label>();
                entity.labels.ForEach(p =>
                {
                    labels.Add(new Label()
                    {
                        name = p.text,
                        style = p.style
                    });
                });
                await _labelServices.InsertAsync(labels);
            }

            var items = await _labelServices.QueryListByClauseAsync(p => names.Contains(p.name));
            var idsInts = items.Select(p => p.id).ToArray();
            var ids = String.Join(",", idsInts);

            var bl = await base.BaseDal.UpdateAsync(p => new Goods() { labelIds = ids }, p => entity.ids.Contains(p.id));

            jm.code = bl ? 0 : 1;
            jm.msg = bl ? "设置成功" : "设置失败";

            return jm;
        }
        #endregion

        #region 取消标签==========================================

        /// <summary>
        /// 取消标签
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> DoDeleteLabel(FmSetLabel entity)
        {
            var jm = new AdminUiCallBack();

            var names = entity.labels.Select(p => p.text).ToList();
            //获取已经存在数据库数据
            var labels = await _labelServices.QueryListByClauseAsync(p => names.Contains(p.name));
            var labelIds = labels.Select(p => p.id).ToList();

            var goods = await base.QueryListByClauseAsync(p => entity.ids.Contains(p.id));
            goods.ForEach(p =>
            {
                if (!string.IsNullOrEmpty(p.labelIds))
                {
                    var ids = CommonHelper.StringToIntArray(p.labelIds);
                    var newIds = ids.Except(labelIds).ToList();
                    if (newIds.Any())
                    {
                        p.labelIds = String.Join(",", newIds);
                    }
                    else
                    {
                        p.labelIds = "";
                    }
                }
            });

            var bl = await base.UpdateAsync(goods);

            jm.code = bl ? 0 : 1;
            jm.msg = bl ? "设置成功" : "设置失败";


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
            return await _dal.GetWeight(productsId);
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

            return _dal.ChangeStock(productsId, type, num);
        }
        #endregion

        #region 获取商品详情

        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="isPromotion">是否涉及优惠</param>
        /// <param name="type"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<Goods> GetGoodsDetial(int id, int userId = 0, bool isPromotion = false, string type = "goods", int groupId = 0)
        {
            var model = await _dal.QueryByIdAsync(id);
            if (model == null) return null;
            //取图片集
            var album = model.images.Split(",");
            model.image = !string.IsNullOrEmpty(model.image) ? model.image : "/static/images/common/empty-banner.png";
            model.album = album;
            //label_ids
            //获取用户信息
            if (userId > 0)
            {
                model.isFav = await _goodsCollectionServices.ExistsAsync(p => p.goodsId == model.id && p.userId == userId);
            }
            //取默认货品
            var products = await _productsServices.QueryByClauseAsync(p => p.goodsId == model.id && p.isDefalut == true && p.isDel == false);
            if (products == null) return null;
            var getProductInfo = await _productsServices.GetProductInfo(products.id, isPromotion, userId, type, groupId);
            if (getProductInfo == null) return null;

            model.product = getProductInfo;

            model.sn = getProductInfo.sn;
            model.price = getProductInfo.price;
            model.costprice = getProductInfo.costprice;
            model.mktprice = getProductInfo.mktprice;
            model.stock = getProductInfo.stock;
            model.freezeStock = getProductInfo.freezeStock;
            model.weight = getProductInfo.weight;

            //获取品牌
            var brand = await _brandServices.QueryByIdAsync(model.brandId);
            model.brand = brand;

            //取出销量
            model.buyCount = await _orderItemServices.GetCountAsync(p => p.goodsId == model.id);
            return model;
        }
        #endregion


        #region 获取随机推荐数据

        /// <summary>
        /// 获取随机推荐数据
        /// </summary>
        /// <param name="number"></param>
        /// <param name="isRecommend"></param>
        /// <returns></returns>
        public async Task<List<Goods>> GetGoodsRecommendList(int number, bool isRecommend = false)
        {
            return await _dal.GetGoodsRecommendList(number, isRecommend);
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
            return await _dal.GetCountAsync(predicate, blUseNoLock);
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
            return await _dal.QueryPageAsync(predicate, orderByExpression, orderByType, pageIndex, pageSize, blUseNoLock);
        }
        #endregion


        #region 重写根据条件查询一定数量数据
        /// <summary>
        ///     重写根据条件查询一定数量数据
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
            return await _dal.QueryListByClauseAsync(predicate, take, orderByPredicate, orderByType, blUseNoLock);
        }
        #endregion



        #region 重写根据条件查询数据
        /// <summary>
        ///     重写根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序字段，如name asc,age desc</param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns>泛型实体集合</returns>
        public new async Task<List<Goods>> QueryListByClauseAsync(Expression<Func<Goods, bool>> predicate, string orderBy = "",
            bool blUseNoLock = false)
        {
            return await _dal.QueryListByClauseAsync(predicate, orderBy, blUseNoLock);
        }
        #endregion


        #region 重写根据条件查询分页数据
        /// <summary>
        ///     重写根据条件查询分页数据
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
            return await _dal.QueryPageAsync(predicate, orderBy, pageIndex, pageSize, blUseNoLock);
        }
        #endregion


        /// <summary>
        ///     根据条件查询代理池商品分页数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public async Task<IPageList<Goods>> QueryAgentGoodsPageAsync(Expression<Func<Goods, bool>> predicate, string orderBy = "",
            int pageIndex = 1, int pageSize = 20, bool blUseNoLock = false)
        {
            return await _dal.QueryAgentGoodsPageAsync(predicate, orderBy, pageIndex, pageSize, blUseNoLock);
        }


        #region 获取下拉商品数据
        /// <summary>
        ///     获取下拉商品数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<EnumEntity>> QueryEnumEntityAsync()
        {

            return await _dal.QueryEnumEntityAsync();
        }
        #endregion


    }
}
