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
    ///     组织机构表 接口实现
    /// </summary>
    public class SysOrganizationRepository : BaseRepository<SysOrganization>, ISysOrganizationRepository
    {
        public SysOrganizationRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}