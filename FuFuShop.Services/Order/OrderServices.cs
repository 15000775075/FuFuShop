using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Caching;
using FuFuShop.Common.Extensions;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository;
using FuFuShop.Services.BaseServices;
using FuFuShop.Services.Bill;
using FuFuShop.Services.Good;
using FuFuShop.Services.Shop;
using FuFuShop.Services.User;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;

namespace FuFuShop.Services
{
    /// <summary>
    /// 订单表 接口实现
    /// </summary>
    public class OrderServices : BaseServices<Order>, IOrderServices
    {
        private readonly IOrderRepository _dal;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IShipServices _shipServices;
        private readonly ICartServices _cartServices;
        private readonly IGoodsServices _goodsServices;
        private readonly IBillDeliveryServices _billDeliveryServices;
        private readonly IAreaServices _areaServices;
        private readonly ISettingServices _settingServices;
        private readonly ILogisticsServices _logisticsServices;
        private readonly IBillAftersalesServices _billAftersalesServices;
        private readonly IOrderItemServices _orderItemServices;
        private readonly IOrderLogServices _orderLogServices;
        private readonly IUserShipServices _userShipServices;
        private readonly IStoreServices _storeServices;
        private readonly IUserServices _userServices;
        private readonly IBillPaymentsServices _billPaymentsServices;
        private readonly IPaymentsServices _paymentsServices;
        private readonly IBillRefundServices _billRefundServices;
        private readonly IBillLadingServices _billLadingServices;
        private readonly IBillReshipServices _billReshipServices;
        private readonly IMessageCenterServices _messageCenterServices;
        private readonly IGoodsCommentServices _goodsCommentServices;
        private readonly ISysTaskLogServices _taskLogServices;
        private readonly IPromotionRecordServices _promotionRecordServices;
        private readonly IRedisOperationRepository _redisOperationRepository;

        public OrderServices(IOrderRepository dal
            , IHttpContextAccessor httpContextAccessor
            , IShipServices shipServices
            , ICartServices cartServices
            , IGoodsServices goodsServices
            , IBillDeliveryServices billDeliveryServices
            , IAreaServices areaServices
            , ISettingServices settingServices
            , ILogisticsServices logisticsServices
            , IBillAftersalesServices billAftersalesServices
            , IOrderItemServices orderItemServices
            , IOrderLogServices orderLogServices
            , IUserShipServices userShipServices
            , IStoreServices storeServices
            , IUserServices userServices
            , IBillPaymentsServices billPaymentsServices
            , IPaymentsServices paymentsServices
            , IBillRefundServices billRefundServices
            , IBillLadingServices billLadingServices
            , IBillReshipServices billReshipServices
            , IMessageCenterServices messageCenterServices
            , IGoodsCommentServices goodsCommentServices
            , ISysTaskLogServices taskLogServices
            , IRedisOperationRepository redisOperationRepository)
        {
            this._dal = dal;
            base.BaseDal = dal;

            _httpContextAccessor = httpContextAccessor;
            _shipServices = shipServices;
            _cartServices = cartServices;
            _goodsServices = goodsServices;

            _billDeliveryServices = billDeliveryServices;
            _areaServices = areaServices;
            _settingServices = settingServices;
            _logisticsServices = logisticsServices;
            _billAftersalesServices = billAftersalesServices;
            _orderItemServices = orderItemServices;
            _orderLogServices = orderLogServices;
            _userShipServices = userShipServices;
            _storeServices = storeServices;
            _userServices = userServices;
            _billPaymentsServices = billPaymentsServices;
            _paymentsServices = paymentsServices;
            _billRefundServices = billRefundServices;
            _billLadingServices = billLadingServices;
            _billReshipServices = billReshipServices;
            _messageCenterServices = messageCenterServices;
            _goodsCommentServices = goodsCommentServices;
            _taskLogServices = taskLogServices;
            _promotionRecordServices = promotionRecordServices;
            _redisOperationRepository = redisOperationRepository;
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

        #region 获取单个订单所有详情
        /// <summary>
        /// 根据订单编号获取单个订单所有详情
        /// </summary>
        /// <returns></returns>
        public async Task<WebApiCallBack> GetOrderInfoByOrderId(string id, int userId = 0, int aftersaleLevel = 0)
        {
            var jm = new WebApiCallBack();

            var order = new Order();
            order = userId > 0
                ? await _dal.QueryByClauseAsync(p => p.orderId == id && p.userId == userId)
                : await _dal.QueryByClauseAsync(p => p.orderId == id);
            if (order == null)
            {
                jm.msg = "获取订单失败";
                return jm;
            }
            //订单详情(子货品数据)
            order.items = await _orderItemServices.QueryListByClauseAsync(p => p.orderId == order.orderId);

            if (order.items.Any())
            {
                order.items.ForEach(p =>
                {
                    if (!string.IsNullOrEmpty(p.promotionList))
                    {
                        var jobj = JObject.Parse(p.promotionList);
                        if (jobj.Values().Any())
                        {
                            p.promotionObj = jobj.Values().FirstOrDefault();
                        }
                    }
                });
            }

            //获取相关状态描述说明转换
            order.statusText = EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.OrderStatus>(order.status);
            order.payStatusText = EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.OrderPayStatus>(order.payStatus);
            order.shipStatusText = EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.OrderShipStatus>(order.shipStatus);
            order.sourceText = EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.Source>(order.source);
            order.typeText = EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.OrderType>(order.orderType);
            order.confirmStatusText = EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.OrderConfirmStatus>(order.confirmStatus);
            order.taxTypeText = EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.OrderTaxType>(order.taxType);
            order.paymentCodeText = EnumHelper.GetEnumDescriptionByKey<GlobalEnumVars.PaymentsTypes>(order.paymentCode);
            //获取日志
            order.orderLog = await _orderLogServices.QueryListByClauseAsync(p => p.orderId == order.orderId);

            if (order.orderLog.Any())
            {
                order.orderLog.ForEach(p =>
                {
                    p.typeText = EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.OrderLogTypes>(p.type);
                });
            }

            //用户信息
            order.user = await _userServices.QueryByIdAsync(order.userId);
            if (order.user != null)
            {
                order.user.passWord = "";
            }
            //支付单
            order.paymentItem = await _billPaymentsServices.QueryListByClauseAsync(p => p.sourceId == order.orderId);
            //退款单
            order.refundItem = await _billRefundServices.QueryListByClauseAsync(p => p.sourceId == order.orderId);
            //提货单
            order.ladingItem = await _billLadingServices.QueryListByClauseAsync(p => p.orderId == order.orderId);
            //退货单
            order.returnItem = await _billReshipServices.QueryListByClauseAsync(p => p.orderId == order.orderId);
            //售后单
            order.aftersalesItem = await _billAftersalesServices.QueryListByClauseAsync(p => p.orderId == order.orderId);
            //发货单
            order.delivery = await _billDeliveryServices.QueryListByClauseAsync(p => p.orderId == order.orderId);
            if (order.delivery != null && order.delivery.Any())
            {
                foreach (var item in order.delivery)
                {
                    var outFirstAsync = await _logisticsServices.QueryByClauseAsync(p => p.logiCode == item.logiCode);
                    item.logiName = outFirstAsync != null ? outFirstAsync.logiName : item.logiCode;
                }
            }
            //获取提货门店
            if (order.storeId != 0)
            {
                order.store = await _storeServices.QueryByIdAsync(order.storeId);
                if (order.store != null)
                {
                    var areaBack = await _areaServices.GetAreaFullName(order.store.areaId);
                    order.store.allAddress = areaBack.status ? areaBack.data + order.store.address : order.store.address;
                }
            }
            //获取配送方式
            if (order.logisticsId > 0)
            {
                order.logistics = await _shipServices.QueryByIdAsync(order.logisticsId);
            }
            //获取订单状态及中文描述
            order.globalStatus = GetGlobalStatus(order);

            order.globalStatusText = EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.OrderAllStatusType>(order.globalStatus);
            //收货地区三级地址
            var shipAreaBack = await _areaServices.GetAreaFullName(order.shipAreaId);

            order.shipAreaName = shipAreaBack.status ? shipAreaBack.data.ToString() : "";

            //获取支付方式
            var pm = await _paymentsServices.QueryByClauseAsync(p => p.code == order.paymentCode);
            order.paymentName = pm != null ? pm.name : "未知支付方式";
            //优惠券
            //if (!string.IsNullOrEmpty(order.coupon))
            //{
            //    order.couponObj = await _couponServices.QueryWithAboutAsync(p => p.usedId == order.orderId);
            //}
            //  order.couponObj = await _couponServices.QueryWithAboutAsync(p => p.usedId == order.orderId);

            var allConfigs = await _settingServices.GetConfigDictionaries();
            //获取该状态截止时间
            switch (order.globalStatus)
            {
                case (int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_PAYMENT: ////待付款
                    var cancelTime = CommonHelper.GetConfigDictionary(allConfigs, SystemSettingConstVars.OrderCancelTime).ObjectToInt(1) * 86400;
                    var dt = order.createTime.AddSeconds(cancelTime);
                    order.remainingTime = dt;
                    order.remaining = CommonHelper.GetRemainingTime(dt);
                    break;
                case (int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_RECEIPT: //待收货
                    var autoSignTime = CommonHelper.GetConfigDictionary(allConfigs, SystemSettingConstVars.OrderAutoSignTime).ObjectToInt(1) * 86400;
                    var dtautoSignTime = order.createTime.AddSeconds(autoSignTime);
                    order.remainingTime = dtautoSignTime;
                    order.remaining = CommonHelper.GetRemainingTime(dtautoSignTime);
                    break;
                case (int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_EVALUATE:  //待评价
                    var autoEvalTime = CommonHelper.GetConfigDictionary(allConfigs, SystemSettingConstVars.OrderAutoEvalTime).ObjectToInt(1) * 86400;
                    var dtautoEvalTime = order.createTime.AddSeconds(autoEvalTime);
                    order.remainingTime = dtautoEvalTime;
                    order.remaining = CommonHelper.GetRemainingTime(dtautoEvalTime);
                    break;

                default:
                    order.remaining = string.Empty;
                    order.remainingTime = null;
                    break;

            }
            //支付单
            if (order.paymentItem != null && order.paymentItem.Any())
            {
                foreach (var item in order.paymentItem)
                {
                    item.paymentCodeName = EnumHelper.GetEnumDescriptionByKey<GlobalEnumVars.PaymentsTypes>(item.paymentCode);
                    item.statusName = EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.BillPaymentsStatus>(item.status);
                }
            }
            //退款单
            if (order.refundItem != null && order.refundItem.Any())
            {
                foreach (var item in order.refundItem)
                {
                    item.paymentCodeName = EnumHelper.GetEnumDescriptionByKey<GlobalEnumVars.PaymentsTypes>(item.paymentCode);
                    item.statusName = EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.BillRefundStatus>(item.status);
                }
            }
            //发货单
            if (order.delivery != null && order.delivery.Any())
            {
                foreach (var item in order.delivery)
                {
                    var logisticsModel = await _logisticsServices.GetLogiInfo(item.logiCode);
                    if (logisticsModel.status)
                    {
                        var logisticsData = logisticsModel.data as Logistics;
                        item.logiName = logisticsData.logiName;
                    }
                    var areaModel = await _areaServices.GetAreaFullName(item.shipAreaId);
                    if (areaModel.status)
                    {
                        item.shipAreaIdName = areaModel.data as string;
                    }
                }
            }
            //提货单
            if (order.ladingItem != null && order.ladingItem.Any())
            {
                foreach (var item in order.ladingItem)
                {
                    var storeModel = await _storeServices.QueryByIdAsync(item.storeId);
                    item.storeName = storeModel != null ? storeModel.storeName : "";
                    item.statusName = EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.BillLadingStatus>(item.status ? 2 : 1);

                    if (item.clerkId != 0)
                    {
                        var userModel = await _userServices.QueryByIdAsync(item.clerkId);
                        if (userModel != null)
                        {
                            item.clerkIdName = !string.IsNullOrEmpty(userModel.nickName) ? userModel.nickName : userModel.mobile;
                        }
                    }
                }
            }
            //退货单
            if (order.returnItem != null && order.returnItem.Any())
            {
                foreach (var item in order.returnItem)
                {
                    var logisticsModel = await _logisticsServices.GetLogiInfo(item.logiCode);
                    if (logisticsModel.status)
                    {
                        var logisticsData = logisticsModel.data as Logistics;
                        item.logiName = logisticsData.logiName;
                    }
                    item.statusName = EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.BillReshipStatus>(item.status);
                }
            }
            //售后单取当前活动的收货单
            if (order.aftersalesItem != null && order.aftersalesItem.Any())
            {
                foreach (var item in order.aftersalesItem)
                {
                    order.billAftersalesId = item.aftersalesId;
                    //如果售后单里面有待审核的活动售后单，那就直接拿这条
                    if (item.status == (int)GlobalEnumVars.BillAftersalesStatus.WaitAudit) break;
                }
            }
            //把退款金额和退货商品查出来
            AfterSalesVal(order, aftersaleLevel);
            //促销信息
            if (!string.IsNullOrEmpty(order.promotionList))
            {
                order.promotionObj = JsonConvert.DeserializeObject(order.promotionList);
            }


            jm.status = true;
            jm.data = order;
            jm.msg = GlobalConstVars.GetDataSuccess;

            return jm;
        }

        #endregion



        /// <summary>
        /// 订单数量统计
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> OrderCount(int type = 0, int userId = 0)
        {
            var count = 0;
            var where = GetReverseStatus(type);
            if (userId > 0)
            {
                where = where.And(p => p.userId == userId);
            }

            count = await _dal.GetCountAsync(where);
            return count;

        }


        #region 获取订单状态反查
        /// <summary>
        /// 获取订单状态反查
        /// </summary>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public Expression<Func<Order, bool>> GetReverseStatus(int status)
        {
            var where = PredicateBuilder.True<Order>();
            switch (status)
            {
                case (int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_PAYMENT: //待付款
                    where = where.And(p => p.status == (int)GlobalEnumVars.OrderStatus.Normal);
                    where = where.And(p => p.payStatus == (int)GlobalEnumVars.OrderPayStatus.No);
                    where = where.And(p => p.isdel == false);
                    break;
                case (int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_DELIVERY: //待发货
                    where = where.And(p => p.status == (int)GlobalEnumVars.OrderStatus.Normal);
                    where = where.And(p => p.payStatus != (int)GlobalEnumVars.OrderPayStatus.No);
                    where = where.And(p => p.shipStatus == (int)GlobalEnumVars.OrderShipStatus.No || p.shipStatus == (int)GlobalEnumVars.OrderShipStatus.PartialYes);
                    where = where.And(p => p.isdel == false);
                    break;
                case (int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_RECEIPT: //待收货
                    where = where.And(p => p.status == (int)GlobalEnumVars.OrderStatus.Normal);
                    where = where.And(p => p.payStatus != (int)GlobalEnumVars.OrderPayStatus.No);
                    where = where.And(p => p.shipStatus == (int)GlobalEnumVars.OrderShipStatus.Yes || p.shipStatus == (int)GlobalEnumVars.OrderShipStatus.PartialYes);
                    where = where.And(p => p.confirmStatus == (int)GlobalEnumVars.OrderConfirmStatus.ReceiptNotConfirmed);
                    where = where.And(p => p.isdel == false);
                    break;
                case (int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_EVALUATE: //待评价
                    where = where.And(p => p.status == (int)GlobalEnumVars.OrderStatus.Normal);
                    where = where.And(p => p.payStatus != (int)GlobalEnumVars.OrderPayStatus.No);
                    where = where.And(p => p.shipStatus != (int)GlobalEnumVars.OrderShipStatus.No);
                    where = where.And(p => p.confirmStatus == (int)GlobalEnumVars.OrderConfirmStatus.ConfirmReceipt);
                    where = where.And(p => p.isComment == false);
                    where = where.And(p => p.isdel == false);
                    break;
                case (int)GlobalEnumVars.OrderAllStatusType.ALL_COMPLETED_EVALUATE: //已评价
                    where = where.And(p => p.status == (int)GlobalEnumVars.OrderStatus.Normal);
                    where = where.And(p => p.payStatus != (int)GlobalEnumVars.OrderPayStatus.No);
                    where = where.And(p => p.shipStatus != (int)GlobalEnumVars.OrderShipStatus.No);
                    where = where.And(p => p.confirmStatus == (int)GlobalEnumVars.OrderConfirmStatus.ConfirmReceipt);
                    where = where.And(p => p.isComment == true);
                    where = where.And(p => p.isdel == false);
                    break;
                case (int)GlobalEnumVars.OrderAllStatusType.ALL_CANCEL: //已取消
                    where = where.And(p => p.status == (int)GlobalEnumVars.OrderStatus.Cancel);
                    where = where.And(p => p.isdel == false);
                    break;
                case (int)GlobalEnumVars.OrderAllStatusType.ALL_COMPLETED: //已完成
                    where = where.And(p => p.status == (int)GlobalEnumVars.OrderStatus.Complete);
                    where = where.And(p => p.isdel == false);
                    break;
                default:
                    where = where.And(p => p.isdel == false);
                    break;
            }
            return where;
        }

        #endregion

        #region 订单支付

        /// <summary>
        /// 订单支付
        /// </summary>
        /// <param name="orderId">订单编号</param>
        /// <param name="paymentCode">支付方式</param>
        /// <param name="billPaymentInfo">支付单据</param>
        /// <returns></returns>
        public async Task<WebApiCallBack> Pay(string orderId, string paymentCode, BillPayments billPaymentInfo)
        {
            var jm = new WebApiCallBack() { msg = "订单支付失败" };

            //获取订单
            var order = await _dal.QueryByClauseAsync(p => p.orderId == orderId && p.status == (int)GlobalEnumVars.OrderStatus.Normal);
            if (order == null)
            {
                return jm;
            }
            if (order.payStatus == (int)GlobalEnumVars.OrderPayStatus.Yes || order.payStatus == (int)GlobalEnumVars.OrderPayStatus.PartialNo || order.payStatus == (int)GlobalEnumVars.OrderPayStatus.Refunded)
            {
                jm.msg = "订单" + orderId + "支付失败，订单已经支付";
                jm.data = order;
            }
            else
            {
                //赋值，用于传递完整数据到事件处理中
                order.payedAmount = order.orderAmount;
                order.paymentTime = DateTime.Now;
                order.updateTime = DateTime.Now;
                order.paymentCode = paymentCode;
                order.payStatus = (int)GlobalEnumVars.OrderPayStatus.Yes;

                var isUpdate = await _dal.UpdateAsync(
                    p => new Order()
                    {
                        paymentCode = paymentCode,
                        payStatus = (int)GlobalEnumVars.OrderPayStatus.Yes,
                        paymentTime = order.paymentTime,
                        payedAmount = order.orderAmount,
                        updateTime = order.updateTime
                    }, p => p.orderId == order.orderId);
                jm.data = isUpdate;

                if (isUpdate)
                {
                    order.payStatus = (int)GlobalEnumVars.OrderPayStatus.Yes;
                    jm.status = true;
                    jm.msg = "订单支付成功";


                    //如果是门店自提，应该自动跳过发货，生成提货单信息，使用提货单核销。
                    if (order.receiptType == (int)GlobalEnumVars.OrderReceiptType.SelfDelivery)
                    {
                        var allConfigs = await _settingServices.GetConfigDictionaries();
                        var storeOrderAutomaticDelivery = CommonHelper
                            .GetConfigDictionary(allConfigs, SystemSettingConstVars.StoreOrderAutomaticDelivery)
                            .ObjectToInt(1);
                        if (storeOrderAutomaticDelivery == 1)
                        {
                            //订单自动发货
                            await _redisOperationRepository.ListLeftPushAsync(RedisMessageQueueKey.OrderAutomaticDelivery, JsonConvert.SerializeObject(order));
                        }
                    }

                    //发送支付成功信息,增加发送内容
                    await _messageCenterServices.SendMessage(order.userId, GlobalEnumVars.PlatformMessageTypes.OrderPayed.ToString(), JObject.FromObject(order));
                    await _messageCenterServices.SendMessage(order.userId, GlobalEnumVars.PlatformMessageTypes.SellerOrderNotice.ToString(), JObject.FromObject(order));

                    //用户升级处理
                    await _redisOperationRepository.ListLeftPushAsync(RedisMessageQueueKey.UserUpGrade, JsonConvert.SerializeObject(order));

                }
            }
            //订单记录
            var orderLog = new OrderLog
            {
                orderId = order.orderId,
                userId = order.userId,
                type = (int)GlobalEnumVars.OrderLogTypes.LOG_TYPE_PAY,
                msg = jm.msg,
                data = JsonConvert.SerializeObject(jm),
                createTime = DateTime.Now
            };
            await _orderLogServices.InsertAsync(orderLog);

            return jm;
        }
        #endregion

        #region 获取订单全局状态
        /// <summary>
        /// 获取订单全局状态
        /// </summary>
        /// <param name="orderInfo">订单数据</param>
        /// <returns></returns>
        public static int GetGlobalStatus(Order orderInfo)
        {
            var status = 0;
            if (orderInfo.status == (int)GlobalEnumVars.OrderStatus.Complete)
            {
                status = (int)GlobalEnumVars.OrderAllStatusType.ALL_COMPLETED; //已完成
            }
            else if (orderInfo.status == (int)GlobalEnumVars.OrderStatus.Cancel)
            {
                status = (int)GlobalEnumVars.OrderAllStatusType.ALL_CANCEL; //已取消
            }
            else if (orderInfo.status == (int)GlobalEnumVars.OrderStatus.Normal)
            {
                if (orderInfo.payStatus == (int)GlobalEnumVars.OrderPayStatus.No)
                {
                    status = (int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_PAYMENT;//待付款
                }
                else
                {
                    if (orderInfo.shipStatus == (int)GlobalEnumVars.OrderShipStatus.No || orderInfo.shipStatus == (int)GlobalEnumVars.OrderShipStatus.PartialYes)
                    {
                        status = (int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_DELIVERY;//待发货

                    }
                    else if ((orderInfo.shipStatus == (int)GlobalEnumVars.OrderShipStatus.Yes || orderInfo.shipStatus == (int)GlobalEnumVars.OrderShipStatus.PartialYes) && orderInfo.confirmStatus == (int)GlobalEnumVars.OrderConfirmStatus.ReceiptNotConfirmed)
                    {
                        status = (int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_RECEIPT;//待收货

                    }
                    else if (orderInfo.shipStatus != (int)GlobalEnumVars.OrderShipStatus.No && orderInfo.confirmStatus == (int)GlobalEnumVars.OrderConfirmStatus.ConfirmReceipt && orderInfo.isComment == false)
                    {
                        status = (int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_EVALUATE;//待评价
                    }
                    else if (orderInfo.shipStatus != (int)GlobalEnumVars.OrderShipStatus.No && orderInfo.confirmStatus == (int)GlobalEnumVars.OrderConfirmStatus.ConfirmReceipt && orderInfo.isComment == true)
                    {
                        status = (int)GlobalEnumVars.OrderAllStatusType.ALL_COMPLETED_EVALUATE;//已评价

                    }
                }
            }
            return status;
        }
        #endregion

        #region 把退款的金额和退货的商品数量保存起来
        /// <summary>
        /// 把退款的金额和退货的商品数量保存起来
        /// </summary>
        /// <param name="order"></param>
        /// <param name="aftersaleLevel">取售后单的时候，售后单的等级，0：待审核的和审核通过的售后单，1未审核的，2审核通过的</param>
        public void AfterSalesVal(Order order, int aftersaleLevel)
        {
            var addAftersalesStatus = false;
            var res = _billAftersalesServices.OrderToAftersales(order.orderId, aftersaleLevel);
            var resData = res.data as OrderToAftersalesDto;
            //已经退过款的金额
            order.refunded = resData.refundMoney;
            //算退货商品数量
            foreach (var item in order.items)
            {
                if (resData.reshipGoods.ContainsKey(item.id))
                {
                    item.reshipNums = resData.reshipGoods[item.id].reshipNums;
                    item.reshipedNums = resData.reshipGoods[item.id].reshipedNums;

                    //商品总数量 - 已发货数量 - 未发货的退货数量（总退货数量减掉已发货的退货数量）
                    if (!addAftersalesStatus && (item.nums - item.reshipNums) > 0)//如果没退完，就可以再次发起售后
                    {
                        addAftersalesStatus = true;
                    }
                }
                else
                {
                    item.reshipNums = 0;  //退货商品
                    item.reshipedNums = 0;//已发货的退货商品
                    if (!addAftersalesStatus) //没退货，就能发起售后
                    {
                        addAftersalesStatus = true;
                    }
                }
            }
            //商品没退完或没退，可以发起售后，但是订单状态不对的话，也不能发起售后
            if (order.payStatus == (int)GlobalEnumVars.OrderPayStatus.No || order.status != (int)GlobalEnumVars.OrderStatus.Normal)
            {
                addAftersalesStatus = false;
            }
            order.addAftersalesStatus = addAftersalesStatus;
        }

        #endregion
    }
}