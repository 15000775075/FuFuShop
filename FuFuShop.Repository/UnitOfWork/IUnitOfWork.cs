
using SqlSugar;

namespace FuFuShop.Repository.UnitOfWork
{
    public interface IUnitOfWork
    {
        SqlSugarScope GetDbClient();
        void BeginTran();
        void CommitTran();
        void RollbackTran();
    }
}