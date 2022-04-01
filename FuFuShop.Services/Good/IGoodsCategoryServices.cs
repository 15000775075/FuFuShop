using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;

namespace CoreCms.Net.IServices
{
    /// <summary>
    ///     商品分类 服务工厂接口
    /// </summary>
    public interface IGoodsCategoryServices : IBaseServices<GoodsCategory>
    {
        #region 获取缓存的所有数据==========================================================

        /// <summary>
        ///     获取缓存的所有数据
        /// </summary>
        /// <returns></returns>
        Task<List<GoodsCategory>> GetCaChe();

        #endregion


        /// <summary>
        ///     判断商品分类下面是否有某一个商品分类
        /// </summary>
        /// <param name="catParentId"></param>
        /// <param name="catId"></param>
        /// <returns></returns>
        Task<bool> IsChild(int catParentId, int catId);


        /// <summary>
        ///     判断是否含有子类
        /// </summary>
        /// <param name="list"></param>
        /// <param name="catParentId"></param>
        /// <param name="catId"></param>
        /// <returns></returns>
        bool IsHave(List<GoodsCategory> list, int catParentId, int catId);

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