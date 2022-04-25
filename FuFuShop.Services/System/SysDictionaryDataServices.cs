

using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;


namespace FuFuShop.Services
{
    /// <summary>
    /// 数据字典项表 接口实现
    /// </summary>
    public class SysDictionaryDataServices : BaseServices<SysDictionaryData>, ISysDictionaryDataServices
    {
        private readonly ISysDictionaryDataRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public SysDictionaryDataServices(IUnitOfWork unitOfWork, ISysDictionaryDataRepository dal)
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }


    }
}
