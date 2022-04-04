using FuFuShop.Model.Entities.User;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Repository.User;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services.User
{
    /// <summary>
    /// 用户地址表 接口实现
    /// </summary>
    public class UserShipServices : BaseServices<UserShip>, IUserShipServices
    {
        private readonly IUserShipRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public UserShipServices(IUnitOfWork unitOfWork, IUserShipRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 重写异步插入方法
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public new async Task<WebApiCallBack> InsertAsync(UserShip entity)
        {
            return await _dal.InsertAsync(entity);
        }



        /// <summary>
        /// 重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> UpdateAsync(UserShip entity)
        {
            return await _dal.UpdateAsync(entity);
        }
    }
}
