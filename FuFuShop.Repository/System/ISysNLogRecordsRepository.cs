
using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
namespace FuFuShop.Repository
{
    /// <summary>
    ///     Nlog记录表 工厂接口
    /// </summary>
    public interface ISysNLogRecordsRepository : IBaseRepository<SysNLogRecords>
    {
        #region 重写增删改查操作===========================================================

        ///// <summary>
        ///// 事务重写异步插入方法
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //new Task<AdminUiCallBack> InsertAsync(SysNLogRecords entity);


        ///// <summary>
        ///// 重写异步更新方法方法
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //new Task<AdminUiCallBack> UpdateAsync(SysNLogRecords entity);


        ///// <summary>
        ///// 重写异步更新方法方法
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //new Task<AdminUiCallBack> UpdateAsync(List<SysNLogRecords> entity);


        ///// <summary>
        ///// 重写删除指定ID的数据
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //new Task<AdminUiCallBack> DeleteByIdAsync(object id);


        ///// <summary>
        ///// 重写删除指定ID集合的数据(批量删除)
        ///// </summary>
        ///// <param name="ids"></param>
        ///// <returns></returns>
        //new Task<AdminUiCallBack> DeleteByIdsAsync(int[] ids);

        #endregion
    }
}