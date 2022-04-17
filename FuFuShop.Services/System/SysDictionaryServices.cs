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
using FuFuShop.Services;
using FuFuShop.Services.BaseServices;


namespace CoreCms.Net.Services
{
    /// <summary>
    /// 数据字典表 接口实现
    /// </summary>
    public class SysDictionaryServices : BaseServices<SysDictionary>, ISysDictionaryServices
    {
        private readonly ISysDictionaryRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public SysDictionaryServices(IUnitOfWork unitOfWork, ISysDictionaryRepository dal)
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }


    }
}