using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;

namespace CoreCms.Net.IRepository
{
    /// <summary>
    ///     商品分类 工厂接口
    /// </summary>
    public interface IGoodsCategoryRepository : IBaseRepository<GoodsCategory>
    {
        #region 获取缓存的所有数据==========================================================

        /// <summary>
        ///     获取缓存的所有数据
        /// </summary>
        /// <returns></returns>
        Task<List<GoodsCategory>> GetCaChe();

        #endregion

        #region 重写增删改查操作===========================================================

        /// <summary>
        ///     事务重写异步插入方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> InsertAsync(GoodsCategory entity);


        /// <summary>
        ///     重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> UpdateAsync(GoodsCategory entity);


        /// <summary>
        ///     重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> UpdateAsync(List<GoodsCategory> entity);


        /// <summary>
        ///     重写删除指定ID的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> DeleteByIdAsync(object id);


        /// <summary>
        ///     重写删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> DeleteByIdsAsync(int[] ids);

        #endregion
    }
}