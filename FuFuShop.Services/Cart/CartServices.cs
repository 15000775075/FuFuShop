using FuFuShop.Model.Entitys;
using FuFuShop.Repository;
using FuFuShop.Services.BaseServices;


namespace FuFuShop.Services
{
    /// <summary>
    /// 购物车表 接口实现
    /// </summary>
    public class CartServices : BaseServices<Cart>, ICartServices
    {
        private readonly ICartRepository _dal;

        //private readonly IGoodsCollectionServices _goodsCollectionServices;
        //private readonly IPinTuanRuleServices _pinTuanRuleServices;
        //private readonly IShipServices _shipServices;
        //private readonly IPromotionServices _promotionServices;
        //private readonly IPromotionConditionServices _promotionConditionServices;
        //private readonly IPromotionResultServices _promotionResultServices;
        //private readonly ICouponServices _couponServices;
        //private readonly IUserServices _userServices;
        //private readonly ISettingServices _settingServices;
        private readonly IServiceProvider _serviceProvider;
        //private readonly IProductsServices _productsServices;
        //private readonly IPinTuanGoodsServices _pinTuanGoodsServices;
        //private readonly IPinTuanRecordServices _pinTuanRecordServices;
        //private readonly IGoodsServices _goodsServices;
        //private readonly IGoodsCategoryServices _goodsCategoryServices;

        public CartServices(
            ICartRepository dal
            , IServiceProvider serviceProvider
            //, IGoodsCollectionServices goodsCollectionServices
            //, IPinTuanRuleServices pinTuanRuleServices
            //, IShipServices shipServices
            //, IPromotionServices promotionServices
            //, ICouponServices couponServices
            //, IUserServices userServices
            //, ISettingServices settingServices
            //, IProductsServices productsServices
            //, IPinTuanGoodsServices pinTuanGoodsServices,
            //IPromotionConditionServices promotionConditionServices,
            //IGoodsServices goodsServices, 
            //IGoodsCategoryServices goodsCategoryServices,
            //IPromotionResultServices promotionResultServices,
            //IPinTuanRecordServices pinTuanRecordServices

            )
        {
            _dal = dal;
            BaseDal = dal;

            _serviceProvider = serviceProvider;
            //_goodsCollectionServices = goodsCollectionServices;
            //_pinTuanRuleServices = pinTuanRuleServices;
            //_shipServices = shipServices;
            //_promotionServices = promotionServices;
            //_couponServices = couponServices;
            //_userServices = userServices;
            //_settingServices = settingServices;
            //_productsServices = productsServices;
            //_pinTuanGoodsServices = pinTuanGoodsServices;
            //_promotionConditionServices = promotionConditionServices;
            //_goodsServices = goodsServices;
            //_goodsCategoryServices = goodsCategoryServices;
            //_promotionResultServices = promotionResultServices;
            //_pinTuanRecordServices = pinTuanRecordServices;
        }

    }
}
