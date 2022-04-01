using CoreCms.Net.IServices;
using FuFuShop.IRepository;
using FuFuShop.Model.Entities;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;


namespace FuFuShop.Services
{
    /// <summary>
    /// 商品表 接口实现
    /// </summary>
    public class GoodsServices : BaseServices<Goods>, IGoodsServices
    {
        private readonly IGoodsRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductsServices _productsServices;
        private readonly IGoodsCollectionServices _goodsCollectionServices;


        public GoodsServices(IUnitOfWork unitOfWork, IProductsServices productsServices, IGoodsCollectionServices goodsCollectionServices, IGoodsRepository dal
        )
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
            _productsServices = productsServices;
            _goodsCollectionServices = goodsCollectionServices;

        }

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



            //取出销量
            //model.buyCount = await _orderItemServices.GetCountAsync(p => p.goodsId == model.id);
            return model;
        }




    }
}
