
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    /// 商品类型属性表 接口实现
    /// </summary>
    public class GoodsTypeSpecServices : BaseServices<GoodsTypeSpec>, IGoodsTypeSpecServices
    {
        private readonly IGoodsTypeSpecRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public GoodsTypeSpecServices(IUnitOfWork unitOfWork, IGoodsTypeSpecRepository dal)
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        ///     使用事务重写异步插入方法
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> InsertAsync(FmGoodsTypeSpecInsert entity)
        {
            return await _dal.InsertAsync(entity);
        }

        /// <summary>
        /// 重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> UpdateAsync(FmGoodsTypeSpecUpdate entity)
        {
            return await _dal.UpdateAsync(entity);
        }


        /// <summary>
        /// 重写删除指定ID的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> DeleteByIdAsync(object id)
        {
            return await _dal.DeleteByIdAsync(id);
        }


    }
}
