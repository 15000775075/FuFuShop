using FuFuShop.Model.Entities.User;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services.User
{
    /// <summary>
    ///     用户地址表 服务工厂接口
    /// </summary>
    public interface IUserShipServices : IBaseServices<UserShip>
    {
        /// <summary>
        ///     事务重写异步插入方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        new Task<WebApiCallBack> InsertAsync(UserShip entity);


        /// <summary>
        ///     重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> UpdateAsync(UserShip entity);
    }
}