using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository;
using FuFuShop.Services.BaseServices;
using FuFuShop.Services.Good;
using FuFuShop.Services.Shop;
using FuFuShop.Services.User;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FuFuShop.Services
{
    /// <summary>
    /// 订单表 接口实现
    /// </summary>
    public class OrderServices : BaseServices<Order>, IOrderServices
    {
        private readonly IOrderRepository _dal;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderItemServices _orderItemServices;
        private readonly ILogServices _orderLogServices;
        private readonly ICartServices _cartServices;
        private readonly IGoodsServices _goodsServices;
        private readonly ILogisticsServices _logisticsServices;
        private readonly IUserShipServices _userShipServices;
        private readonly IShipServices _shipServices;

        public OrderServices(IOrderRepository dal
            , IHttpContextAccessor httpContextAccessor
            , IOrderItemServices orderItemServices
            , ILogServices orderLogServices,
            ICartServices cartServices,
             IGoodsServices goodsServices,
              ILogisticsServices logisticsServices,
              IUserShipServices userShipServices,
               IShipServices shipServices)
        {
            _dal = dal;
            BaseDal = dal;
            _httpContextAccessor = httpContextAccessor;
            _orderItemServices = orderItemServices;
            _orderLogServices = orderLogServices;
            _cartServices = cartServices;
            _goodsServices = goodsServices;
            _logisticsServices = logisticsServices;
            _userShipServices = userShipServices;
            _shipServices = shipServices;
        }
        #region 创建订单

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="userId">用户序列</param>
        /// <param name="orderType">订单类型，1是普通订单，2是拼团订单</param>
        /// <param name="cartIds">购物车货品序列</param>
        /// <param name="receiptType">收货方式,1快递物流，2同城配送，3门店自提</param>
        /// <param name="ushipId">用户地址库序列</param>
        /// <param name="storeId">门店序列</param>
        /// <param name="ladingName">提货人姓名</param>
        /// <param name="ladingMobile">提货人联系方式</param>
        /// <param name="memo">备注</param>
        /// <param name="point">积分</param>
        /// <param name="couponCode">优惠券码</param>
        /// <param name="source">来源平台</param>
        /// <param name="scene">场景值（一般小程序才有）</param>
        /// <param name="taxType">发票信息</param>
        /// <param name="taxName">发票抬头</param>
        /// <param name="taxCode">发票税务编码</param>
        /// <param name="objectId">关联非普通订单营销功能的序列</param>
        /// <param name="teamId">拼团订单分组序列</param>
        /// <returns></returns>
        public async Task<WebApiCallBack> ToAdd(int userId, int orderType, string cartIds, int receiptType, int ushipId, int storeId, string ladingName, string ladingMobile, string memo, int point, string couponCode, int source, int scene, int taxType, string taxName, string taxCode, int objectId, int teamId)
        {
            var jm = new WebApiCallBack() { methodDescription = "创建订单" };

            var order = new Order
            {
                orderId = CommonHelper.GetSerialNumberType((int)GlobalEnumVars.SerialNumberType.订单编号),
                userId = userId,
                orderType = orderType,
                point = point,
                coupon = couponCode,
                receiptType = receiptType,
                objectId = objectId
            };

            //生成收货信息
            var areaId = 0;
            var deliveryRes = await FormatOrderDelivery(order, receiptType, ushipId, storeId, ladingName, ladingMobile);
            if (!deliveryRes.status)
            {
                return deliveryRes;
            }
            else
            {
                areaId = Convert.ToInt32(deliveryRes.data);
            }

            //通过购物车生成订单信息和订单明细信息
            List<OrderItem> orderItems;
            var ids = CommonHelper.StringToIntArray(cartIds);
            var orderRes = await FormatOrder(order, userId, ids, areaId, point, couponCode, false, receiptType, objectId);
            if (!orderRes.status)
            {
                return orderRes;
            }
            else
            {
                orderItems = orderRes.data as List<OrderItem>;
            }

            //以下值不是通过购物车得来的，是直接赋值的，就写这里吧，不写formatOrder里了。
            order.memo = memo;
            order.source = source;
            order.taxType = taxType;
            order.taxTitle = taxName;
            order.taxCode = taxCode;
            order.shipStatus = (int)GlobalEnumVars.OrderShipStatus.No;
            order.status = (int)GlobalEnumVars.OrderStatus.Normal;
            order.confirmStatus = (int)GlobalEnumVars.OrderConfirmStatus.ReceiptNotConfirmed;
            order.createTime = DateTime.Now;

            //开始事务处理
            await _dal.InsertAsync(order);

            //上面保存好订单表，下面保存订单的其他信息
            if (orderItems != null)
            {
                jm.msg = "更改库存";
                //更改库存
                foreach (var item in orderItems)
                {
                    var res = _goodsServices.ChangeStock(item.productId, GlobalEnumVars.OrderChangeStockType.order.ToString(), item.nums);
                    if (res.status == false)
                    {

                        jm.msg = "更新库存数据失败";
                        return jm;
                    }
                }
                jm.msg = "订单明细更新" + orderItems.Count;
                var outItems = await _orderItemServices.InsertCommandAsync(orderItems);
                var outItemsBool = outItems > 0;
                if (outItemsBool == false)
                {

                    jm.msg = "订单明细更新失败";
                    jm.data = outItems;
                    return jm;
                }

            }

            //清除购物车信息
            await _cartServices.DeleteAsync(p => ids.Contains(p.id) && p.userId == userId && p.type == orderType);

            //订单记录
            var orderLog = new OrderLog
            {
                userId = userId,
                orderId = order.orderId,
                type = (int)GlobalEnumVars.OrderLogTypes.LOG_TYPE_CREATE,
                msg = "订单创建",
                data = JsonConvert.SerializeObject(order),
                createTime = DateTime.Now
            };
            await _orderLogServices.InsertAsync(orderLog);

            //0元订单记录支付成功
            if (order.orderAmount <= 0)
            {
                orderLog = new OrderLog
                {
                    userId = userId,
                    orderId = order.orderId,
                    type = (int)GlobalEnumVars.OrderLogTypes.LOG_TYPE_PAY,
                    msg = "0元订单直接支付成功",
                    data = JsonConvert.SerializeObject(order),
                    createTime = DateTime.Now
                };
                await _orderLogServices.InsertAsync(orderLog);
            }


            order.taxTitle = taxName;
            order.taxCode = taxCode;



            jm.status = true;
            jm.data = order;

            return jm;
        }

        #endregion

        #region 生成订单的收货信息
        /// <summary>
        /// 生成订单的收货信息
        /// </summary>
        /// <param name="order">订单信息</param>
        /// <param name="receiptType">收货方式,1快递物流，2同城配送，3门店自提</param>
        /// <param name="ushipId">用户地址库序列</param>
        /// <param name="storeId">门店序列</param>
        /// <param name="ladingName">提货人姓名</param>
        /// <param name="ladingMobile">提货人联系方式</param>
        /// <returns></returns>
        private async Task<WebApiCallBack> FormatOrderDelivery(Order order, int receiptType, int ushipId, int storeId, string ladingName, string ladingMobile)
        {
            var res = new WebApiCallBack() { methodDescription = "生成订单的收货信息" };
            var areaId = 0;

            //快递邮寄
            var userShipInfo = await _userShipServices.QueryByClauseAsync(p => p.userId == order.userId && p.id == ushipId);
            if (userShipInfo == null)
            {
                res.data = 11050;
                res.msg = GlobalErrorCodeVars.Code11050;
                return res;
            }
            areaId = userShipInfo.areaId;

            //快递邮寄
            order.shipAreaId = userShipInfo.areaId;
            order.shipAddress = userShipInfo.address;
            order.shipName = userShipInfo.name;
            order.shipMobile = userShipInfo.mobile;

            var ship = _shipServices.GetShip(userShipInfo.areaId);
            if (ship != null)
            {
                order.logisticsId = ship.id;
                order.logisticsName = ship.name;
                order.storeId = 0;
            }

            res.status = true;
            res.msg = "订单的收货信息生成成功";
            res.data = areaId;

            return res;
        }
        #endregion

        #region 生成订单的时候，根据购物车信息生成订单信息及明细信息

        /// <summary>
        /// 生成订单的时候，根据购物车信息生成订单信息及明细信息
        /// </summary>
        /// <param name="order">订单数组</param>
        /// <param name="userId">用户id</param>
        /// <param name="cartIds">购物车信息</param>
        /// <param name="areaId">收货地区</param>
        /// <param name="point">使用积分</param>
        /// <param name="couponCode">使用优惠券</param>
        /// <param name="freeFreight">是否包邮</param>
        /// <param name="deliveryType">收货方式,1快递物流，2同城配送，3门店自提</param>
        /// <param name="groupId">团队明细</param>
        /// <returns>返回订单明细信息</returns>
        private async Task<WebApiCallBack> FormatOrder(Order order, int userId, int[] cartIds, int areaId, int point,
            string couponCode, bool freeFreight = false, int deliveryType = (int)GlobalEnumVars.OrderReceiptType.Logistics, int groupId = 0)
        {
            var res = new WebApiCallBack() { methodDescription = "生成订单信息及明细信息" };

            var cartModel = await _cartServices.GetCartInfos(userId, cartIds, order.orderType, areaId, point, couponCode,
           freeFreight, deliveryType, groupId);
            if (!cartModel.status)
            {
                return cartModel;
            }

            if (cartModel.data is CartDto cartDto)
            {
                order.goodsAmount = cartDto.goodsAmount;
                order.orderAmount = cartDto.amount;
                if (order.orderAmount == 0)
                {
                    order.payStatus = (int)GlobalEnumVars.OrderPayStatus.Yes;
                    order.paymentTime = DateTime.Now;
                }
                else
                {
                    order.payStatus = (int)GlobalEnumVars.OrderPayStatus.No;
                }
                order.costFreight = cartDto.costFreight;

                order.point = cartDto.point;
                order.pointMoney = cartDto.pointExchangeMoney;
                order.weight = cartDto.weight;
                order.orderDiscountAmount = cartDto.orderPromotionMoney > 0 ? cartDto.orderPromotionMoney : 0;
                order.goodsDiscountAmount = cartDto.goodsPromotionMoney > 0 ? cartDto.goodsPromotionMoney : 0;
                order.couponDiscountAmount = cartDto.couponPromotionMoney;
                order.ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress != null ? _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() : "127.0.0.1";
                //以上保存了订单主体表信息，以下生成订单明细表
                var items = FormatOrderItems(cartDto.list, order.orderId);
                if (!items.Any())
                {
                    res.status = false;
                    res.data = 10000;
                    res.msg = GlobalErrorCodeVars.Code10000;
                    return res;
                }
                res.status = true;
                res.data = items;
            }

            return res;
        }

        #endregion    #region 根据购物车的明细生成订单明细

        /// <summary>
        /// 根据购物车的明细生成订单明细
        /// </summary>
        private static List<OrderItem> FormatOrderItems(List<CartProducts> list, string orderId)
        {
            var res = new List<OrderItem>();
            foreach (var item in list)
            {
                if (item.isSelect == false) continue;
                var model = new OrderItem
                {
                    orderId = orderId,
                    goodsId = item.products.goodsId,
                    productId = item.products.id,
                    sn = item.products.sn,
                    bn = item.products.bn,
                    name = item.products.name,
                    price = item.products.price,
                    costprice = item.products.costprice,
                    mktprice = item.products.mktprice,
                    imageUrl = item.products.images,
                    nums = item.nums,
                    amount = item.products.amount,
                    promotionAmount = item.products.promotionAmount > 0 ? item.products.promotionAmount : 0,
                    weight = Math.Round(item.weight * item.nums, 2),
                    sendNums = 0,
                    addon = item.products.spesDesc,
                    createTime = DateTime.Now
                };
                if (item.products.promotionList.Count > 0)
                {
                    var promotionList = new Dictionary<int, WxNameTypeDto>();
                    foreach (var proDto in item.products.promotionList)
                    {
                        if (proDto.Value.type == 2)
                        {
                            promotionList.Add(proDto.Key, proDto.Value);
                        }
                    }
                    model.promotionList = JsonConvert.SerializeObject(promotionList);
                }
                res.Add(model);
            }
            return res;
        }

        public Task<WebApiCallBack> GetOrderInfoByOrderId(string id, int userId = 0, int aftersaleLevel = 0)
        {
            throw new NotImplementedException();
        }
    }
}