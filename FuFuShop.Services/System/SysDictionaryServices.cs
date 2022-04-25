
using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;


namespace FuFuShop.Services
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
