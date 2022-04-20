using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Repository.User;
using FuFuShop.Services.BaseServices;
using Microsoft.AspNetCore.Http;

namespace FuFuShop.Services.User
{
    /// <summary>
    /// 用户表 接口实现
    /// </summary>
    public class UserServices : BaseServices<FuFuShopUser>, IUserServices
    {
        private readonly IUserRepository _dal;

        private readonly IUnitOfWork _unitOfWork;
        // private readonly PermissionRequirement _permissionRequirement;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //   private readonly IUserLogServices _userLogServices;

        public UserServices(IUnitOfWork unitOfWork,
            IUserRepository dal,
             IHttpContextAccessor httpContextAccessor
            //   IUserLogServices userLogServices,
            //  PermissionRequirement permissionRequirement
            )
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;

            // _permissionRequirement = permissionRequirement;
            _httpContextAccessor = httpContextAccessor;
            // _userLogServices = userLogServices;
        }



        /// <summary>
        /// 按天统计新会员
        /// </summary>
        /// <returns></returns>
        public async Task<List<StatisticsOut>> Statistics(int day)
        {
            return await _dal.Statistics(day);
        }

        /// <summary>
        /// 按天统计当天下单活跃会员
        /// </summary>
        /// <returns></returns>
        public async Task<List<StatisticsOut>> StatisticsOrder(int day)
        {

            return await _dal.StatisticsOrder(day);
        }


    }
}
