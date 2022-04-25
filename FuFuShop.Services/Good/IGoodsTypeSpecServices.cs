
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     商品类型属性表 服务工厂接口
    /// </summary>
    public interface IGoodsTypeSpecServices : IBaseServices<GoodsTypeSpec>
    {
        #region 重写增删改查操作===========================================================

        /// <summary>
        ///     重写异步插入方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<AdminUiCallBack> InsertAsync(FmGoodsTypeSpecInsert entity);


        /// <summary>
        ///     重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<AdminUiCallBack> UpdateAsync(FmGoodsTypeSpecUpdate entity);


        ///// <summary>
        ///// 重写异步更新方法方法
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //new Task<AdminUiCallBack> UpdateAsync(List<GoodsTypeSpec> entity);


        /// <summary>
        ///     重写删除指定ID的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> DeleteByIdAsync(object id);


        ///// <summary>
        ///// 重写删除指定ID集合的数据(批量删除)
        ///// </summary>
        ///// <param name="ids"></param>
        ///// <returns></returns>
        //new Task<AdminUiCallBack> DeleteByIdsAsync(int[] ids);

        #endregion
    }
}