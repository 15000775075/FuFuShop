/***********************************************************************
 *            Project: CoreCms
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www.corecms.net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/1/31 21:45:10
 *        Description: 暂无
 ***********************************************************************/

using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Repository.BaseRepository;
namespace CoreCms.Net.Repository
{
    /// <summary>
    ///     定时任务日志 接口实现
    /// </summary>
    public class SysTaskLogRepository : BaseRepository<SysTaskLog>, ISysTaskLogRepository
    {
        public SysTaskLogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}