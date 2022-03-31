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

        //private readonly ICoreCmsGoodsCollectionServices _goodsCollectionServices;
        //private readonly ICoreCmsPinTuanRuleServices _pinTuanRuleServices;
        //private readonly ICoreCmsShipServices _shipServices;
        //private readonly ICoreCmsPromotionServices _promotionServices;
        //private readonly ICoreCmsPromotionConditionServices _promotionConditionServices;
        //private readonly ICoreCmsPromotionResultServices _promotionResultServices;
        //private readonly ICoreCmsCouponServices _couponServices;
        //private readonly ICoreCmsUserServices _userServices;
        //private readonly ICoreCmsSettingServices _settingServices;
        private readonly IServiceProvider _serviceProvider;
        //private readonly ICoreCmsProductsServices _productsServices;
        //private readonly ICoreCmsPinTuanGoodsServices _pinTuanGoodsServices;
        //private readonly ICoreCmsPinTuanRecordServices _pinTuanRecordServices;
        //private readonly ICoreCmsGoodsServices _goodsServices;
        //private readonly ICoreCmsGoodsCategoryServices _goodsCategoryServices;

        public CartServices(
            ICartRepository dal
            , IServiceProvider serviceProvider
            //, ICoreCmsGoodsCollectionServices goodsCollectionServices
            //, ICoreCmsPinTuanRuleServices pinTuanRuleServices
            //, ICoreCmsShipServices shipServices
            //, ICoreCmsPromotionServices promotionServices
            //, ICoreCmsCouponServices couponServices
            //, ICoreCmsUserServices userServices
            //, ICoreCmsSettingServices settingServices
            //, ICoreCmsProductsServices productsServices
            //, ICoreCmsPinTuanGoodsServices pinTuanGoodsServices,
            //ICoreCmsPromotionConditionServices promotionConditionServices,
            //ICoreCmsGoodsServices goodsServices, 
            //ICoreCmsGoodsCategoryServices goodsCategoryServices,
            //ICoreCmsPromotionResultServices promotionResultServices,
            //ICoreCmsPinTuanRecordServices pinTuanRecordServices

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
