using FuFuShop.Common.AppSettings;
using FuFuShop.Model.Entities.Cart;
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository;
using FuFuShop.Services.BaseServices;
using FuFuShop.Services.Good;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace FuFuShop.Services
{
    /// <summary>
    /// 购物车表 接口实现
    /// </summary>
    public class CartServices : BaseServices<Cart>, ICartServices
    {
        private readonly ICartRepository _dal;

        private readonly IGoodsCollectionServices _goodsCollectionServices;
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
            , IGoodsCollectionServices goodsCollectionServices
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
            _goodsCollectionServices = goodsCollectionServices;
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

        /// <summary>
        /// 添加单个货品到购物车
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="productId">货品序号</param>
        /// <param name="nums">数量</param>
        /// <param name="numType">数量类型/1是直接增加/2是赋值</param>
        /// <param name="cartTypes">1普通购物还是2团购秒杀3团购模式4秒杀模式6砍价模式7赠品</param>
        /// <param name="objectId">关联对象类型</param>
        /// <returns></returns>
        public async Task<WebApiCallBack> Add(int userId, int productId, int nums, int numType, int cartTypes = 1, int objectId = 0)
        {
            var jm = new WebApiCallBack();

            using var container = _serviceProvider.CreateScope();

            var orderServices = container.ServiceProvider.GetService<IOrderServices>();
            var productsServices = container.ServiceProvider.GetService<IProductsServices>();
            var goodsServices = container.ServiceProvider.GetService<IGoodsServices>();

            //获取数据 
            if (nums == 0)
            {
                jm.msg = "请选择货品数量";
                return jm;
            }
            if (productId == 0)
            {
                jm.msg = "请选择货品";
                return jm;
            }
            //获取货品信息
            var products = await productsServices.GetProductInfo(productId, false, userId); //第二个参数是不算促销信息,否则促销信息就算重复了
            if (products == null)
            {
                jm.msg = "获取货品信息失败";
                return jm;
            }
            //判断是否下架
            var isMarketable = await productsServices.GetShelfStatus(productId);
            if (isMarketable == false)
            {
                jm.msg = "货品已下架";
                return jm;
            }
            //剩余库存可购判定
            var canBuyNum = products.stock;
            //获取是否存在记录
            var catInfo = await _dal.QueryByClauseAsync(p => p.userId == userId && p.productId == productId && p.objectId == objectId);

            if (catInfo == null)
            {
                if (nums > canBuyNum)
                {
                    jm.msg = "库存不足";
                    return jm;
                }

                catInfo = new Cart
                {
                    userId = userId,
                    productId = productId,
                    nums = nums,
                    type = cartTypes,
                    objectId = objectId
                };
                var outId = await _dal.InsertAsync(catInfo);
                jm.status = outId > 0;
                jm.data = outId;
            }
            else
            {
                if (numType == 1)
                {
                    catInfo.nums = nums + catInfo.nums;
                    if (catInfo.nums > canBuyNum)
                    {
                        jm.msg = "库存不足";
                        return jm;
                    }
                }
                else
                {
                    catInfo.nums = nums;
                }
                jm.status = await _dal.UpdateAsync(catInfo);
                jm.data = catInfo.id;
            }
            jm.msg = jm.status ? "添加成功" : "添加失败";

            return jm;
        }


        /// <summary>
        /// 重写删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<WebApiCallBack> DeleteByIdsAsync(int id, int userId)
        {
            var jm = new WebApiCallBack();

            if (userId == 0)
            {
                jm.msg = "用户信息获取失败";
                return jm;
            }
            if (id <= 0)
            {
                jm.msg = "请提交要删除的货品";
                return jm;
            }
            jm.status = await _dal.DeleteAsync(p => p.id == id && p.userId == userId);
            jm.msg = jm.status ? "删除成功" : "删除失败";

            return jm;
        }

        /// <summary>
        /// 获取购物车原始列表(未核算)
        /// </summary>
        /// <param name="userId">用户序号</param>
        /// <param name="ids">已选择货号</param>
        /// <param name="type">购物车类型/同订单类型</param>
        /// <param name="objectId">关联非订单类型数据序列</param>
        /// <returns></returns>
        public async Task<WebApiCallBack> GetCartDtoData(int userId, int[] ids = null, int type = 1, int objectId = 0)
        {
            var jm = new WebApiCallBack() { methodDescription = "获取购物车原始列表(未核算)" };

            using var container = _serviceProvider.CreateScope();
            var productsService = container.ServiceProvider.GetService<IProductsServices>();
            var goodsServices = container.ServiceProvider.GetService<IGoodsServices>();


            var carts = await _dal.QueryListByClauseAsync(p => p.userId == userId && p.type == type, p => p.id, OrderByType.Asc);
            var cartDto = new CartDto { userId = userId, type = type };

            foreach (var item in carts)
            {
                var cartProducts = new CartProducts();
                //如果没有此商品，就在购物车里删掉
                var productInfo = await productsService.GetProductInfo(item.productId, false, userId);
                if (productInfo == null)
                {
                    await _dal.DeleteAsync(item);
                    continue;
                }
                //商品下架，就从购物车里面删除
                var ps = await productsService.GetShelfStatus(item.productId);
                if (ps == false)
                {
                    await _dal.DeleteAsync(item);
                    continue;
                }
                //获取重量
                // var goodsWeight = await goodsServices.GetWeight(item.productId);

                //开始赋值
                cartProducts.id = item.id;
                cartProducts.userId = userId;
                cartProducts.productId = item.productId;
                cartProducts.nums = item.nums;
                cartProducts.type = item.type;
                // cartProducts.weight = goodsWeight;
                cartProducts.products = productInfo;
                //如果传过来了购物车数据，就算指定的购物车的数据，否则，就算全部购物车的数据
                if (ids != null && ids.Any() && ids.Contains(item.id))
                {
                    cartProducts.isSelect = true;
                }
                else
                {
                    cartProducts.isSelect = false;
                }
                //判断商品是否已收藏
                cartProducts.isCollection = await _goodsCollectionServices.Check(userId, cartProducts.products.goodsId);

                cartDto.list.Add(cartProducts);
            }
            jm.status = true;
            jm.data = cartDto;
            jm.msg = GlobalConstVars.GetDataSuccess;

            return jm;
        }


        /// <summary>
        /// 获取处理后的购物车信息
        /// </summary>
        /// <param name="userId">用户序列</param>
        /// <param name="ids">选中的购物车商品</param>
        /// <param name="orderType">订单类型</param>
        /// <param name="areaId">收货地址id</param>
        /// <param name="point">消费的积分</param>
        /// <param name="couponCode">优惠券码</param>
        /// <param name="freeFreight">是否免运费</param>
        /// <param name="deliveryType">关联上面的是否免运费/1=快递配送（要去算运费）生成订单记录快递方式  2=门店自提（不需要计算运费）生成订单记录门店自提信息</param>
        /// <param name="objectId">关联非普通订单营销类型序列</param>
        /// <returns></returns>
        public async Task<WebApiCallBack> GetCartInfos(int userId, int[] ids, int orderType, int areaId, int point, string couponCode, bool freeFreight = false, int deliveryType = (int)GlobalEnumVars.OrderReceiptType.Logistics, int objectId = 0)
        {
            var jm = new WebApiCallBack() { methodDescription = "获取处理后的购物车信息" };
            var cartDto = new CartDto(); //必须初始化
            var cartDtoData = await GetCartDtoData(userId, ids, orderType, objectId);
            if (!cartDtoData.status)
            {
                jm.msg = "1";
                return cartDtoData;
            }
            cartDto = cartDtoData.data as CartDto;

            jm.msg = "2";

            //算订单总金额
            foreach (var item in cartDto.list)
            {
                jm.msg = "3";
                //库存不足不计算金额不可以选择
                if (item.nums > item.products.stock)
                {
                    item.isSelect = false;
                }
                //单条商品总价
                item.products.amount = Math.Round(item.nums * item.products.price, 2);

                if (item.isSelect)
                {
                    //算订单总商品价格
                    cartDto.goodsAmount = Math.Round(cartDto.goodsAmount + item.products.amount, 2);
                    //算订单总价格
                    cartDto.amount = Math.Round(cartDto.amount + item.products.amount, 2);
                    //计算总重量
                    cartDto.weight = Math.Round(cartDto.weight + Math.Round(item.weight * item.nums, 2), 2);
                }
            }

            //门店订单，强制无运费
            if (deliveryType == (int)GlobalEnumVars.OrderReceiptType.IntraCityService || deliveryType == (int)GlobalEnumVars.OrderReceiptType.SelfDelivery)
            {
                freeFreight = true;
            }

            jm.status = true;
            jm.data = cartDto;
            jm.msg = "4";

            return jm;
        }

        /// <summary>
        /// 设置购物车商品数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nums"></param>
        /// <param name="userId"></param>
        /// <param name="numType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<WebApiCallBack> SetCartNum(int id, int nums, int userId, int numType, int type = 1)
        {
            var jm = new WebApiCallBack();

            if (userId == 0)
            {
                jm.msg = "用户信息获取失败";
                return jm;
            }
            if (id == 0)
            {
                jm.msg = "请提交要设置的信息";
                return jm;
            }
            var cartModel = await _dal.QueryByClauseAsync(p => p.userId == userId && p.productId == id);
            if (cartModel == null)
            {
                jm.msg = "获取购物车数据失败";
                return jm;
            }
            var outData = await Add(userId, cartModel.productId, nums, numType, type);
            jm.status = outData.status;
            jm.msg = jm.status ? GlobalConstVars.SetDataSuccess : GlobalConstVars.SetDataFailure;
            jm.otherData = outData;

            return jm;
        }

    }
}
