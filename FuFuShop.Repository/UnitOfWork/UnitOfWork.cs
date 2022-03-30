using FuFuShop.Common.Loging;
using NLog;
using SqlSugar;
using SqlSugar.IOC;

namespace FuFuShop.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        //public UnitOfWork(ISqlSugarClient sqlSugarClient)
        //{
        //    _sqlSugarClient = sqlSugarClient;
        //}


        public UnitOfWork()
        {
            _sqlSugarClient = DbScoped.SugarScope;
        }

        /// <summary>
        ///     获取DB，保证唯一性
        /// </summary>
        /// <returns></returns>
        public SqlSugarScope GetDbClient()
        {
            // 必须要as，后边会用到切换数据库操作
            return _sqlSugarClient as SqlSugarScope;
        }

        public void BeginTran()
        {
            GetDbClient().BeginTran();
        }

        public void CommitTran()
        {
            try
            {
                GetDbClient().CommitTran(); //
            }
            catch (Exception ex)
            {
                GetDbClient().RollbackTran();
                NLogUtil.WriteFileLog(LogLevel.Error, LogType.Web, "事务提交异常", "事务提交异常", new Exception("事务提交异常", ex));
                throw;
            }
        }

        public void RollbackTran()
        {
            GetDbClient().RollbackTran();
        }
    }
}