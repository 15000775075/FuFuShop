
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Extensions;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;
using FuFuShop.Services.Shop;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Services
{
    /// <summary>
    /// 店铺店员关联表 接口实现
    /// </summary>
    public class ClerkServices : BaseServices<Clerk>, IClerkServices
    {
        private readonly IClerkRepository _dal;
        private readonly IUserServices _userServices;
        private readonly ISettingServices _settingServices;
        private readonly IUnitOfWork _unitOfWork;
        public ClerkServices(IUnitOfWork unitOfWork, IClerkRepository dal
            , IUserServices userServices
            , ISettingServices settingServices)
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
            _userServices = userServices;
            _settingServices = settingServices;
        }

        /// <summary>
        /// 判断是不是店员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<WebApiCallBack> IsClerk(int userId)
        {
            var jm = new WebApiCallBack();

            var allConfigs = await _settingServices.GetConfigDictionaries();

            var storeSwitch = CommonHelper.GetConfigDictionary(allConfigs, SystemSettingConstVars.StoreSwitch).ObjectToInt(2);
            if (storeSwitch == 1)
            {
                var bl = await base.ExistsAsync(p => p.userId == userId);
                jm.status = true;
                jm.data = bl;
                jm.msg = bl ? "是店员" : "不是店员";
            }
            else
            {
                jm.status = true;
                jm.data = false;
                jm.msg = "未开启到店自提";
            }

            return jm;
        }

        /// <summary>
        ///     获取门店关联用户分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public async Task<IPageList<StoreClerkDto>> QueryStoreClerkDtoPageAsync(Expression<Func<StoreClerkDto, bool>> predicate,
            Expression<Func<StoreClerkDto, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false)
        {
            return await _dal.QueryStoreClerkDtoPageAsync(predicate, orderByExpression, orderByType, pageIndex,
                pageSize, blUseNoLock);
        }




        /// <summary>
        ///     获取单个门店用户数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public async Task<StoreClerkDto> QueryStoreClerkDtoByClauseAsync(Expression<Func<StoreClerkDto, bool>> predicate, bool blUseNoLock = false)
        {
            return await _dal.QueryStoreClerkDtoByClauseAsync(predicate, blUseNoLock);
        }

    }
}
