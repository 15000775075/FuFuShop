using FuFuShop.Common.AppSettings;
using FuFuShop.Model.Entities.User;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository.User
{
    /// <summary>
    /// 用户地址表 接口实现
    /// </summary>
    public class UserShipRepository : BaseRepository<UserShip>, IUserShipRepository
    {
        public UserShipRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        #region 重写异步插入方法

        /// <summary>
        /// 重写异步插入方法
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public new async Task<WebApiCallBack> InsertAsync(UserShip entity)
        {
            var jm = new WebApiCallBack();
            if (entity.isDefault == true)
            {
                await DbClient.Updateable<UserShip>().SetColumns(p => p.isDefault == false).Where(p => p.userId == entity.userId).ExecuteCommandAsync();
            }
            var bl = await DbClient.Insertable(entity).ExecuteReturnIdentityAsync() > 0;
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.CreateSuccess : GlobalConstVars.CreateFailure;

            return jm;
        }

        #endregion

        #region 重写异步更新方法方法

        /// <summary>
        /// 重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> UpdateAsync(UserShip entity)
        {
            var jm = new AdminUiCallBack();

            var oldModel = await DbClient.Queryable<UserShip>().Where(p => p.id == entity.id && p.userId == entity.userId).SingleAsync();
            if (oldModel == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            //事物处理过程开始
            oldModel.areaId = entity.areaId;
            oldModel.address = entity.address;
            oldModel.name = entity.name;
            oldModel.mobile = entity.mobile;
            oldModel.isDefault = entity.isDefault;
            //oldModel.createTime = entity.createTime;
            oldModel.updateTime = entity.updateTime;

            if (oldModel.isDefault)
            {
                await DbClient.Updateable<UserShip>().SetColumns(p => p.isDefault == false).Where(p => p.userId == entity.userId).ExecuteCommandAsync();
            }

            //事物处理过程结束
            var bl = await DbClient.Updateable(oldModel).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;

            return jm;
        }

        #endregion



    }
}
