

using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Services
{
    /// <summary>
    /// 退货单表 接口实现
    /// </summary>
    public class BillReshipServices : BaseServices<BillReship>, IBillReshipServices
    {
        private readonly IBillReshipRepository _dal;
        private readonly IBillReshipItemRepository _billReshipItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public BillReshipServices(IUnitOfWork unitOfWork
            , IBillReshipRepository dal
            , IBillReshipItemRepository billReshipItemRepository
            )
        {
            this._dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
            _billReshipItemRepository = billReshipItemRepository;
        }

        /// <summary>
        /// 创建退货单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="aftersalesId"></param>
        /// <param name="aftersalesItems"></param>
        /// <returns></returns>
        public async Task<WebApiCallBack> ToAdd(int userId, string orderId, string aftersalesId, List<BillAftersalesItem> aftersalesItems)
        {
            var jm = new WebApiCallBack();

            if (aftersalesItems == null || aftersalesItems.Count <= 0)
            {
                jm.msg = GlobalErrorCodeVars.Code13209;
                jm.data = jm.code = 13209;
                return jm;
            }

            var model = new BillReship();
            model.reshipId = CommonHelper.GetSerialNumberType((int)GlobalEnumVars.SerialNumberType.退货单编号);
            model.orderId = orderId;
            model.aftersalesId = aftersalesId;
            model.userId = userId;
            model.status = (int)GlobalEnumVars.BillReshipStatus.待退货;
            model.createTime = DateTime.Now;

            await _dal.InsertAsync(model);

            var list = new List<BillReshipItem>();
            foreach (var item in aftersalesItems)
            {
                var reshipItem = new BillReshipItem();
                reshipItem.reshipId = model.reshipId;
                reshipItem.orderItemsId = item.orderItemsId;
                reshipItem.goodsId = item.goodsId;
                reshipItem.productId = item.productId;
                reshipItem.sn = item.sn;
                reshipItem.bn = item.bn;
                reshipItem.name = item.name;
                reshipItem.imageUrl = item.imageUrl;
                reshipItem.nums = item.nums;
                reshipItem.addon = item.addon;
                reshipItem.createTime = DateTime.Now;
                list.Add(reshipItem);
                //保存退货单明细
            }

            await _billReshipItemRepository.InsertAsync(list);

            jm.status = true;
            jm.data = model;

            return jm;
        }


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
        public async Task<IPageList<BillReship>> QueryPageAsync(Expression<Func<BillReship, bool>> predicate,
            Expression<Func<BillReship, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20)
        {
            return await _dal.QueryPageAsync(predicate, orderByExpression, orderByType, pageIndex, pageSize);
        }
        #endregion



        #region 获取单个数据带导航
        /// <summary>
        /// 获取单个数据带导航
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public async Task<BillReship> GetDetails(Expression<Func<BillReship, bool>> predicate,
            Expression<Func<BillReship, object>> orderByExpression, OrderByType orderByType)
        {
            return await _dal.GetDetails(predicate, orderByExpression, orderByType);
        }
        #endregion


    }
}
