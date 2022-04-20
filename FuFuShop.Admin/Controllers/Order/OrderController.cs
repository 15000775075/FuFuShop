/***********************************************************************
 *            Project: 
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www..net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/1/31 21:45:10
 *        Description: 暂无
 ***********************************************************************/

using FuFuShop.Admin.Filter;
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Caching;
using FuFuShop.Common.Extensions;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using FuFuShop.Services.Bill;
using FuFuShop.Services.Shop;
using FuFuShop.Services.User;
using FuFuShop.Services.WeChat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.ComponentModel;

namespace FuFuShop.Admin.Controllers
{
    /// <summary>
    /// 订单表
    ///</summary>
    [Description("订单表")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RequiredErrorForAdmin]
    [Authorize(Permissions.Name)]
    public class OrderController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IOrderServices _OrderServices;
        private readonly IUserServices _userServices;
        private readonly IAreaServices _areaServices;
        private readonly IBillAftersalesServices _aftersalesServices;
        private readonly IBillPaymentsServices _billPaymentsServices;
        private readonly IBillDeliveryServices _billDeliveryServices;
        private readonly IStoreServices _storeServices;
        private readonly ILogisticsServices _logisticsServices;
        private readonly IPaymentsServices _paymentsServices;
        private readonly ISettingServices _settingServices;
        private readonly IUserWeChatInfoServices _userWeChatInfoServices;
        private readonly IRedisOperationRepository _redisOperationRepository;


        private readonly IOrderItemServices _orderItemServices;



        /// <summary>
        /// 构造函数
        ///</summary>
        public OrderController(IWebHostEnvironment webHostEnvironment
            , IOrderServices OrderServices
            , IUserServices userServices
            , IAreaServices areaServices
            , IBillAftersalesServices aftersalesServices
            , IStoreServices storeServices
            , ILogisticsServices logisticsServices
            , IBillPaymentsServices billPaymentsServices
            , IPaymentsServices paymentsServices
            , ISettingServices settingServices, IUserWeChatInfoServices userWeChatInfoServices, IRedisOperationRepository redisOperationRepository, IBillDeliveryServices billDeliveryServices, IOrderItemServices orderItemServices)
        {
            _webHostEnvironment = webHostEnvironment;
            _OrderServices = OrderServices;
            _userServices = userServices;
            _areaServices = areaServices;
            _aftersalesServices = aftersalesServices;
            _storeServices = storeServices;
            _logisticsServices = logisticsServices;
            _billPaymentsServices = billPaymentsServices;
            _paymentsServices = paymentsServices;
            _settingServices = settingServices;
            _userWeChatInfoServices = userWeChatInfoServices;
            _redisOperationRepository = redisOperationRepository;
            _billDeliveryServices = billDeliveryServices;
            _orderItemServices = orderItemServices;
        }

        #region 获取列表============================================================
        // POST: Api/Order/GetPageList
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("获取列表")]
        public async Task<AdminUiCallBack> GetPageList()
        {
            var jm = new AdminUiCallBack();
            var pageCurrent = Request.Form["page"].FirstOrDefault().ObjectToInt(1);
            var pageSize = Request.Form["limit"].FirstOrDefault().ObjectToInt(30);
            var where = PredicateBuilder.True<Order>();
            //获取排序字段

            //订单号 nvarchar
            var orderId = Request.Form["orderId"].FirstOrDefault();
            if (!string.IsNullOrEmpty(orderId))
            {
                where = where.And(p => p.orderId.Contains(orderId));
            }

            //订单状态 int
            var status = Request.Form["status"].FirstOrDefault().ObjectToInt(0);
            if (status > 0)
            {
                where = where.And(p => p.status == status);
            }
            //订单类型 int
            var orderType = Request.Form["orderType"].FirstOrDefault().ObjectToInt(0);
            if (orderType > 0)
            {
                where = where.And(p => p.orderType == orderType);
            }
            //发货状态 int
            var shipStatus = Request.Form["shipStatus"].FirstOrDefault().ObjectToInt(0);
            if (shipStatus > 0)
            {
                where = where.And(p => p.shipStatus == shipStatus);
            }
            //支付状态 int
            var payStatus = Request.Form["payStatus"].FirstOrDefault().ObjectToInt(0);
            if (payStatus > 0)
            {
                where = where.And(p => p.payStatus == payStatus);
            }
            //支付方式代码 nvarchar
            var paymentCode = Request.Form["paymentCode"].FirstOrDefault();
            if (!string.IsNullOrEmpty(paymentCode))
            {
                where = where.And(p => p.paymentCode.Contains(paymentCode));
            }
            //售后状态 int
            var confirmStatus = Request.Form["confirmStatus"].FirstOrDefault().ObjectToInt(0);
            if (confirmStatus > 0)
            {
                where = where.And(p => p.confirmStatus == confirmStatus);
            }
            //订单来源 int
            var source = Request.Form["source"].FirstOrDefault().ObjectToInt(0);
            if (source > 0)
            {
                where = where.And(p => p.source == source);
            }
            //收货方式 int
            var receiptType = Request.Form["receiptType"].FirstOrDefault().ObjectToInt(0);
            if (receiptType > 0)
            {
                where = where.And(p => p.receiptType == receiptType);
            }

            //收货人姓名 nvarchar
            var shipName = Request.Form["shipName"].FirstOrDefault();
            if (!string.IsNullOrEmpty(shipName))
            {
                where = where.And(p => p.shipName.Contains(shipName));
            }
            //收货人地址 nvarchar
            var shipAddress = Request.Form["shipAddress"].FirstOrDefault();
            if (!string.IsNullOrEmpty(shipAddress))
            {
                where = where.And(p => p.shipAddress.Contains(shipAddress));
            }

            //收货电话 nvarchar
            var shipMobile = Request.Form["shipMobile"].FirstOrDefault();
            if (!string.IsNullOrEmpty(shipMobile))
            {
                where = where.And(p => p.shipMobile.Contains(shipMobile));
            }

            //付款单号 nvarchar
            var paymentId = Request.Form["paymentId"].FirstOrDefault();
            if (!string.IsNullOrEmpty(paymentId))
            {
                where = where.And(p => p.shipMobile.Contains(paymentId));
            }

            // datetime
            var createTime = Request.Form["createTime"].FirstOrDefault();
            if (!string.IsNullOrEmpty(createTime))
            {
                if (createTime.Contains("到"))
                {
                    var dts = createTime.Split("到");
                    var dtStart = dts[0].Trim().ObjectToDate();
                    where = where.And(p => p.createTime > dtStart);
                    var dtEnd = dts[1].Trim().ObjectToDate();
                    where = where.And(p => p.createTime < dtEnd);
                }
                else
                {
                    var dt = createTime.ObjectToDate();
                    where = where.And(p => p.createTime > dt);
                }
            }


            //订单状态 int
            var orderUnifiedStatus = Request.Form["orderUnifiedStatus"].FirstOrDefault().ObjectToInt(0);
            if (orderUnifiedStatus > 0)
            {
                if (orderUnifiedStatus == (int)GlobalEnumVars.OrderCountType.payment)
                {
                    //待支付
                    where = where.And(_OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_PAYMENT));
                }
                else if (orderUnifiedStatus == (int)GlobalEnumVars.OrderCountType.delivered)
                {
                    //待发货
                    where = where.And(_OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_DELIVERY));
                }
                else if (orderUnifiedStatus == (int)GlobalEnumVars.OrderCountType.receive)
                {
                    //待收货
                    where = where.And(_OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_RECEIPT));
                }
                else if (orderUnifiedStatus == (int)GlobalEnumVars.OrderCountType.evaluated)
                {
                    //已评价
                    where = where.And(_OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_COMPLETED_EVALUATE));
                }
                else if (orderUnifiedStatus == (int)GlobalEnumVars.OrderCountType.noevaluat)
                {
                    //待评价
                    where = where.And(_OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_EVALUATE));
                }
                else if (orderUnifiedStatus == (int)GlobalEnumVars.OrderCountType.complete)
                {
                    //已完成
                    where = where.And(_OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_COMPLETED));
                }
                else if (orderUnifiedStatus == (int)GlobalEnumVars.OrderCountType.cancel)
                {
                    //已取消
                    where = where.And(_OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_CANCEL));
                }
                else if (orderUnifiedStatus == (int)GlobalEnumVars.OrderCountType.delete)
                {
                    //已取消
                    where = where.And(p => p.isdel == true);
                }
            }
            else
            {
                where = where.And(p => p.isdel == false);
            }

            //获取数据
            var list = await _OrderServices.QueryPageAsync(where, p => p.createTime, OrderByType.Desc, pageCurrent, pageSize);
            if (list != null && list.Any())
            {
                var areaCache = await _areaServices.GetCaChe();
                foreach (var item in list)
                {
                    //item.operating = _OrderServices.GetOperating(item.orderId, item.status, item.payStatus, item.shipStatus, item.receiptType, item.isdel);
                    //item.afterSaleStatus = "";
                    //if (item.aftersalesItem != null && item.aftersalesItem.Any())
                    //{
                    //    foreach (var sale in item.aftersalesItem)
                    //    {
                    //        item.afterSaleStatus += EnumHelper.GetEnumDescriptionByValue<GlobalEnumVars.BillAftersalesStatus>(sale.status) + "<br>";
                    //    }
                    //}
                    var areas = await _areaServices.GetAreaFullName(item.shipAreaId, areaCache);
                    item.shipAreaName = areas.status ? areas.data + "-" + item.shipAddress : item.shipAddress;
                }
            }

            //返回数据
            jm.data = list;
            jm.code = 0;
            jm.count = list.TotalCount;
            jm.msg = "数据调用成功!";
            return jm;
        }
        #endregion

        #region 首页数据============================================================
        // POST: Api/Order/GetIndex
        /// <summary>
        /// 首页数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("首页数据")]
        public async Task<AdminUiCallBack> GetIndex()
        {
            //返回数据
            var jm = new AdminUiCallBack { code = 0 };

            //全部
            var all = await _OrderServices.GetCountAsync(p => p.isdel == false);
            //待支付
            var paymentWhere = _OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_PAYMENT);
            var payment = await _OrderServices.GetCountAsync(paymentWhere);
            //待发货
            var deliveredWhere = _OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_DELIVERY);
            var delivered = await _OrderServices.GetCountAsync(deliveredWhere);
            //待收货
            var receiveWhere = _OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_RECEIPT);
            var receive = await _OrderServices.GetCountAsync(receiveWhere);
            //已评价
            var evaluatedWhere = _OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_COMPLETED_EVALUATE);
            var evaluated = await _OrderServices.GetCountAsync(evaluatedWhere);
            //待评价
            var noevaluatWhere = _OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_EVALUATE);
            var noevaluat = await _OrderServices.GetCountAsync(noevaluatWhere);
            //已完成
            var completeWhere = _OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_COMPLETED);
            var complete = await _OrderServices.GetCountAsync(completeWhere);
            //已取消
            var cancelWhere = _OrderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_CANCEL);
            var cancel = await _OrderServices.GetCountAsync(cancelWhere);
            //删除
            var delete = await _OrderServices.GetCountAsync(p => p.isdel == true);


            //订单状态说明
            var orderStatus = EnumHelper.EnumToList<GlobalEnumVars.OrderStatus>();
            //付款状态
            var payStatus = EnumHelper.EnumToList<GlobalEnumVars.OrderPayStatus>();
            //发货状态
            var shipStatus = EnumHelper.EnumToList<GlobalEnumVars.OrderShipStatus>();
            //订单来源
            var source = EnumHelper.EnumToList<GlobalEnumVars.Source>();
            //订单类型
            var orderType = EnumHelper.EnumToList<GlobalEnumVars.OrderType>();
            //订单支付方式
            var paymentCode = EnumHelper.EnumToList<GlobalEnumVars.PaymentsTypes>();
            //收货状态
            var confirmStatus = EnumHelper.EnumToList<GlobalEnumVars.OrderConfirmStatus>();
            //订单收货方式
            var receiptType = EnumHelper.EnumToList<GlobalEnumVars.OrderReceiptType>();

            jm.data = new
            {
                all,
                payment,
                delivered,
                receive,
                evaluated,
                noevaluat,
                complete,
                cancel,
                delete,
                orderStatus,
                payStatus,
                shipStatus,
                orderType,
                source,
                paymentCode,
                confirmStatus,
                receiptType
            };

            return jm;
        }
        #endregion

        #region 编辑数据============================================================
        // POST: Api/Order/GetEdit
        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("编辑数据")]
        public async Task<AdminUiCallBack> GetEdit([FromBody] FMStringId entity)
        {
            var jm = new AdminUiCallBack();

            var storeList = await _storeServices.QueryAsync();
            var result = await _OrderServices.GetOrderInfoByOrderId(entity.id);
            if (!result.status)
            {
                jm.msg = result.msg;
                return jm;
            }
            jm.code = 0;
            jm.data = new
            {
                orderModel = result.data,
                storeList
            };

            return jm;
        }
        #endregion

        #region 编辑提交============================================================
        // POST: Admins/Order/Edit
        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("编辑提交")]
        public async Task<AdminUiCallBack> DoEdit([FromBody] AdminEditOrderPost entity)
        {
            var jm = new AdminUiCallBack();

            var oldModel = await _OrderServices.QueryByIdAsync(entity.orderId);
            if (oldModel == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            //事物处理过程开始
            if (entity.editType == 1)
            {
                oldModel.shipName = entity.shipName;
                oldModel.shipMobile = entity.shipMobile;
                oldModel.shipAreaId = entity.shipAreaId;
                oldModel.shipAddress = entity.shipAddress;
            }
            else if (entity.editType == 2)
            {
                oldModel.storeId = entity.storeId;
                oldModel.shipName = entity.shipName;
                oldModel.shipMobile = entity.shipMobile;
            }

            if (oldModel.orderAmount != entity.orderAmount && entity.orderAmount > 0)
            {
                oldModel.orderAmount = entity.orderAmount;
            }
            //事物处理过程结束
            var bl = await _OrderServices.UpdateAsync(oldModel);
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;

            return jm;
        }
        #endregion

        #region 发货============================================================
        // POST: Api/Order/GetShip
        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("发货")]
        public async Task<AdminUiCallBack> GetShip([FromBody] FMArrayStringIds entity)
        {
            var jm = new AdminUiCallBack();

            if (entity.id.Length == 0)
            {
                jm.msg = "请选择需要发货的数据";
                return jm;
            }



            var storeList = await _storeServices.QueryAsync();

            var logistics = await _logisticsServices.QueryListByClauseAsync(p => p.isDelete == false);

            var result = await _OrderServices.GetOrderShipInfo(entity.id);
            if (!result.status)
            {
                jm.msg = result.msg;
                return jm;
            }

            if (storeList.Any())
            {
                foreach (var store in storeList)
                {
                    var getfullName = await _areaServices.GetAreaFullName(store.areaId);
                    if (getfullName.status)
                    {
                        store.allAddress = getfullName.data + store.address;
                    }
                }
            }
            jm.code = 0;
            jm.msg = result.msg;
            jm.data = new
            {
                orderModel = result.data,
                storeList,
                logistics,
            };

            return jm;
        }
        #endregion

        #region 发货提交============================================================
        // POST: Admins/Order/Edit
        /// <summary>
        /// 发货提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("发货提交")]
        public async Task<AdminUiCallBack> DoShip([FromBody] AdminOrderShipPost entity)
        {
            var jm = new AdminUiCallBack();

            WebApiCallBack result;
            if (entity.orderId.Contains(","))
            {
                var ids = entity.orderId.Split(",");
                result = await _OrderServices.BatchShip(ids, entity.logiCode, entity.logiNo, entity.items, entity.shipName, entity.shipMobile, entity.shipAddress, entity.memo, entity.storeId, entity.shipAreaId, entity.deliveryCompanyId);
            }
            else
            {
                result = await _OrderServices.Ship(entity.orderId, entity.logiCode, entity.logiNo, entity.items, entity.shipName, entity.shipMobile, entity.shipAddress, entity.memo, entity.storeId, entity.shipAreaId, entity.deliveryCompanyId);
            }

            jm.code = result.status ? 0 : 1;
            jm.msg = result.msg;
            jm.data = result.data;
            jm.otherData = entity;

            return jm;
        }
        #endregion

        #region 秒发货============================================================
        // POST: Admins/Order/Edit
        /// <summary>
        /// 秒发货
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("秒发货")]
        public async Task<AdminUiCallBack> DoSecondsShip([FromBody] FMStringId entity)
        {
            var jm = new AdminUiCallBack();

            var order = await _OrderServices.QueryByIdAsync(entity.id);
            if (order == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }

            var goodItems = await _orderItemServices.QueryListByClauseAsync(p => p.orderId == entity.id);
            if (!goodItems.Any())
            {
                jm.msg = "明细获取失败";
                return jm;
            }

            Dictionary<int, int> items = new Dictionary<int, int>();

            goodItems.ForEach(p =>
            {
                items.Add(p.productId, p.nums);
            });

            var result = new WebApiCallBack();

            if (order.receiptType == (int)GlobalEnumVars.OrderReceiptType.SelfDelivery)
            {
                result = await _OrderServices.Ship(order.orderId, "shangmenziti", "无", items, order.shipName, order.shipMobile, order.shipAddress, order.memo, order.storeId, order.shipAreaId, "OTHERS");
            }
            else if (order.receiptType == (int)GlobalEnumVars.OrderReceiptType.IntraCityService)
            {
                result = await _OrderServices.Ship(order.orderId, "benditongcheng", "无", items, order.shipName, order.shipMobile, order.shipAddress, order.memo, order.storeId, order.shipAreaId, "OTHERS");
            }

            jm.code = result.status ? 0 : 1;
            jm.msg = result.msg;
            jm.data = result.data;
            jm.otherData = entity;

            return jm;
        }
        #endregion

        #region 支付============================================================
        // POST: Api/Order/GetPay
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("支付")]
        public async Task<AdminUiCallBack> GetPay([FromBody] FMArrayStringIds entity)
        {
            var jm = new AdminUiCallBack();

            var type = entity.data.ObjectToInt();
            if (type == 0 || entity.id.Length == 0)
            {
                jm.msg = "请提交合法的数据";
                return jm;
            }

            var result = await _billPaymentsServices.BatchFormatPaymentRel(entity.id, type, null);
            if (result.status == false)
            {
                jm.msg = result.msg;
                jm.data = result.data;
                return jm;
            }
            //取支付方式
            var payments = await _paymentsServices.QueryListByClauseAsync(p => p.isEnable, p => p.sort, OrderByType.Asc);
            jm.code = 0;
            jm.msg = "获取数据成功";
            jm.data = new
            {
                orderId = entity.id,
                type = entity.data,
                payments,
                rel = result.data
            };

            return jm;
        }
        #endregion

        #region 提交支付============================================================
        // POST: Admins/Order/DoToPay
        /// <summary>
        /// 提交支付
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("提交支付")]
        public async Task<AdminUiCallBack> DoToPay([FromBody] AdminOrderDoPayPost entity)
        {
            var jm = new AdminUiCallBack();

            //事物处理过程结束
            var ids = entity.orderId.Split(",");
            var result = await _billPaymentsServices.ToPay(entity.orderId, entity.type, entity.paymentCode);

            jm.code = result.status ? 0 : 1;
            jm.msg = result.msg;

            return jm;
        }
        #endregion

        #region 删除数据============================================================
        // POST: Api/Order/DoDelete/10
        /// <summary>
        /// 单选删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("单选删除")]
        public async Task<AdminUiCallBack> DoDelete([FromBody] FMStringId entity)
        {
            var jm = new AdminUiCallBack();

            var model = await _OrderServices.QueryByIdAsync(entity.id);
            if (model == null)
            {
                jm.msg = GlobalConstVars.DataisNo;
                return jm;
            }
            //假删除
            var bl = await _OrderServices.UpdateAsync(p => new Order() { isdel = true }, p => p.orderId == model.orderId);
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.DeleteSuccess : GlobalConstVars.DeleteFailure;
            return jm;

        }
        #endregion

        #region 还原订单============================================================
        // POST: Api/Order/DoRestore/10
        /// <summary>
        /// 还原订单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("还原订单")]
        public async Task<AdminUiCallBack> DoRestore([FromBody] FMStringId entity)
        {
            var jm = new AdminUiCallBack();

            var model = await _OrderServices.QueryByIdAsync(entity.id);
            if (model == null)
            {
                jm.msg = GlobalConstVars.DataisNo;
                return jm;
            }
            //还原
            var bl = await _OrderServices.UpdateAsync(p => new Order() { isdel = false }, p => p.orderId == model.orderId);
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;
            return jm;

        }
        #endregion

        #region 判断是否存在售后============================================================
        // POST: Api/Order/GetDoHaveAfterSale/10
        /// <summary>
        /// 判断是否存在售后
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("判断是否存在售后")]
        public async Task<AdminUiCallBack> GetDoHaveAfterSale([FromBody] FMStringId entity)
        {
            var jm = new AdminUiCallBack();

            //等待售后审核的订单，不自动操作完成。
            var billAftersalesCount = await _aftersalesServices.GetCountAsync(p => p.orderId == entity.id && p.status == (int)GlobalEnumVars.BillAftersalesStatus.WaitAudit);

            bool bl = billAftersalesCount > 0;

            jm.code = bl ? 0 : 1;
            jm.msg = "存在未处理的售后";

            return jm;

        }
        #endregion

        #region 完成订单============================================================
        // POST: Api/Order/DoComplete/10
        /// <summary>
        /// 完成订单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("完成订单")]
        public async Task<AdminUiCallBack> DoComplete([FromBody] FMStringId entity)
        {
            var jm = new AdminUiCallBack();

            var result = await _OrderServices.CompleteOrder(entity.id);
            jm.code = result.status ? 0 : 1;
            jm.msg = result.msg;
            jm.data = result.data;
            jm.otherData = result.otherData;

            return jm;

        }
        #endregion

        #region 预览数据============================================================
        // POST: Api/Order/GetDetails/10
        /// <summary>
        /// 预览数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("预览数据")]
        public async Task<AdminUiCallBack> GetDetails([FromBody] FMStringId entity)
        {
            var jm = new AdminUiCallBack();

            var result = await _OrderServices.GetOrderInfoByOrderId(entity.id);
            if (result == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            jm.code = result.status ? 0 : 1;
            jm.data = result.data;

            return jm;
        }
        #endregion



        #region 设置是否评论============================================================
        // POST: Api/Order/DoSetisComment/10
        /// <summary>
        /// 设置是否评论
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("设置是否评论")]
        public async Task<AdminUiCallBack> DoSetisComment([FromBody] FMUpdateBoolDataByIntId entity)
        {
            var jm = new AdminUiCallBack();

            var oldModel = await _OrderServices.QueryByIdAsync(entity.id);
            if (oldModel == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            oldModel.isComment = entity.data;

            var bl = await _OrderServices.UpdateAsync(oldModel);
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;

            return jm;
        }
        #endregion

        #region 更新备注============================================================
        // POST: Api/Order/DoSetisdel/10
        /// <summary>
        /// 更新备注
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("更新备注")]
        public async Task<AdminUiCallBack> DoUpdateMark([FromBody] FMStringId entity)
        {
            var jm = new AdminUiCallBack();

            var oldModel = await _OrderServices.QueryByIdAsync(entity.id);
            if (oldModel == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            oldModel.mark = entity.data.ToString();
            var bl = await _OrderServices.UpdateAsync(p => new Order() { mark = oldModel.mark }, p => p.orderId == oldModel.orderId);
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;

            return jm;
        }
        #endregion

        #region 取消订单============================================================
        // POST: Api/Order/CancelOrder/10
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("取消订单")]
        public async Task<AdminUiCallBack> CancelOrder([FromBody] FMArrayStringIds entity)
        {
            var jm = new AdminUiCallBack();

            if (entity.id.Length == 0)
            {
                jm.msg = "请提交要取消的订单号";
                return jm;
            }

            var result = await _OrderServices.CancelOrder(entity.id);
            jm.code = result.status ? 0 : 1;
            jm.msg = result.msg;

            return jm;
        }
        #endregion


        #region 批量删除订单============================================================
        // POST: Api/Order/DeleteOrder/10
        /// <summary>
        /// 批量删除订单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("批量删除订单")]
        public async Task<AdminUiCallBack> DeleteOrder([FromBody] FMArrayStringIds entity)
        {
            var jm = new AdminUiCallBack();

            if (entity.id.Length == 0)
            {
                jm.msg = "请提交要批量删除的订单号";
                return jm;
            }

            var result = await _OrderServices.UpdateAsync(p => new Order() { isdel = true }, p => entity.id.Contains(p.orderId));
            jm.code = result ? 0 : 1;
            jm.msg = result ? "删除成功" : "删除失败";

            return jm;
        }
        #endregion

        #region 重新同步发货============================================================
        // POST: Api/Order/DeleteOrder/10
        /// <summary>
        /// 批量删除订单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("批量删除订单")]
        public async Task<AdminUiCallBack> RefreshDelivery([FromBody] FMStringId entity)
        {
            var jm = new AdminUiCallBack();

            if (string.IsNullOrEmpty(entity.id))
            {
                jm.msg = "请提交要取消的订单号";
                return jm;
            }

            var delivery = await _billDeliveryServices.QueryByClauseAsync(p => p.deliveryId == entity.id);
            if (delivery == null)
            {
                jm.msg = "发货单获取失败";
                return jm;
            }

            jm.code = 0;
            jm.msg = "提交任务成功,请核实远端状态";

            return jm;
        }
        #endregion


        #region 预览快递进度============================================================
        // POST: Api/Order/GetDetails/10
        /// <summary>
        /// 预览快递进度
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("预览快递进度")]
        public async Task<AdminUiCallBack> GetOrderLogistics([FromBody] FMStringId entity)
        {
            var jm = new AdminUiCallBack();

            var result = await _OrderServices.GetOrderInfoByOrderId(entity.id);
            if (result == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            jm.code = result.status ? 0 : 1;
            jm.data = result.data;

            return jm;
        }
        #endregion
    }
}
