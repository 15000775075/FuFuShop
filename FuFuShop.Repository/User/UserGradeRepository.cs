/***********************************************************************
 *            Project: 
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www..net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/1/31 21:45:10
 *        Description: 暂无
 ***********************************************************************/


using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     用户等级表 接口实现
    /// </summary>
    public class UserGradeRepository : BaseRepository<UserGrade>, IUserGradeRepository
    {
        public UserGradeRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}